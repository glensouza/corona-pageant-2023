var pageant = (function () {
    var obsScenes = [];
    var loading = [];
    var pageantLoading = null;
    var pageantMain = null;
    var settingObsComputerScene = "";
    var settingObsCam1Scene = "";
    var settingObsCam2Scene = "";
    var settingObsCam3Scene = "";
    var settingCam1IP = "";
    var settingCam2IP = "";
    var settingCam3IP = "";
    var scriptAct = null;
    var scriptScene = null;
    var scriptSwitchToScene = null;
    var scriptCamera1Action = null;
    var scriptCamera2Action = null;
    var scriptCamera3Action = null;
    var scriptCamera1Position = null;
    var scriptCamera2Position = null;
    var scriptCamera3Position = null;
    var scriptText = null;
    var saveScript = null;
    var theScript = null;
    var fullScript = [];
    var scriptModal = null;

    const obs = new OBSWebSocket();

    const initialize = async function () {
        pageantLoading = $("#pageantLoading");
        pageantMain = $("#pageantMain");
        scriptAct = $("#act");
        scriptScene = $("#scene");
        scriptSwitchToScene = $("#switchToScene");
        scriptCamera1Action = $("#camera1Action");
        scriptCamera2Action = $("#camera2Action");
        scriptCamera3Action = $("#camera3Action");
        scriptCamera1Position = $("#camera1Position");
        scriptCamera2Position = $("#camera2Position");
        scriptCamera3Position = $("#camera3Position");
        scriptText = $("#scriptText");
        saveScript = $("#saveScript");
        saveScript.click(scriptPart);
        theScript = $("#theScript");
        scriptModal = document.getElementById('script');
        scriptModal.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget
            if (button.outerText === 'Edit') {
                var sourceAct = button.getAttribute('data-bs-act')
                var sourceScene = button.getAttribute('data-bs-scene')
                var tempScene = $.grep(fullScript, function (a) {
                    return a.act === sourceAct && a.scene === sourceScene;
                })[0];
                scriptAct.val(tempScene.act);
                scriptAct.prop('disabled', true);
                scriptScene.val(tempScene.scene);
                scriptScene.prop('disabled', true);
                scriptSwitchToScene.val(tempScene.switchToScene);
                scriptCamera1Action.val(tempScene.camera1Action);
                scriptCamera2Action.val(tempScene.camera2Action);
                scriptCamera3Action.val(tempScene.camera3Action);
                scriptCamera1Position.val(tempScene.camera1Position);
                scriptCamera2Position.val(tempScene.camera2Position);
                scriptCamera3Position.val(tempScene.camera3Position);
                scriptText.val(tempScene.text);
            } else {
                scriptAct.val('');
                scriptAct.prop('disabled', false);
                scriptScene.val('');
                scriptScene.prop('disabled', false);
                scriptSwitchToScene.val('');
                scriptCamera1Action.val('');
                scriptCamera2Action.val('');
                scriptCamera3Action.val('');
                scriptCamera1Position.val('');
                scriptCamera2Position.val('');
                scriptCamera3Position.val('');
                scriptText.val('');
            }
        })

        getSettings();
        try {
            showLoading();
            await obs.connect('ws://localhost:4444').then((info) => {
                console.log('Connected and identified', info)
            }, () => {
                console.error('Error Connecting')
            });
            await obs.call('GetSceneList').then(data => {
                $.each(data.scenes, function (index, scene) {
                    obsScenes.push(scene.sceneName);
                });
                hideLoading();
            }, () => {
                hideLoading();
                console.error('Error Connecting')
            });
        } catch (error) {
            hideLoading();
            console.error('Failed to connect', error.code, error.message);
        }
    };

    function showLoading() {
        loading.push(1);
        try {
            pageantLoading.show();
        } catch (error) {
            console.error("error showLoading pageantLoading.show()");
            console.error(error);
        }
        try {
            pageantMain.hide();
        } catch (error) {
            console.error("error showLoading pageantMain.hide()");
            console.error(error);
        }
    }

    function hideLoading() {
        loading.pop();
        if (loading.length === 0) {
            try {
                pageantLoading.hide();
            } catch (error) {
                console.error("error hideLoading pageantLoading.hide()");
                console.error(error);
            }
            try {
                pageantMain.show();
            } catch (error) {
                console.error("error hideLoading pageantMain.show()");
                console.error(error);
            }
        }
    }

    function scriptPart() {
        var act = scriptAct.val();
        var scene = scriptScene.val();
        showLoading();
        const newScene =
        {
            act: act,
            scene: scene,
            text: scriptText.val(),
            camera1Action: scriptCamera1Action.val(),
            camera1Position: scriptCamera1Position.val(),
            camera2Action: scriptCamera2Action.val(),
            camera2Position: scriptCamera2Position.val(),
            camera3Action: scriptCamera3Action.val(),
            camera3Position: scriptCamera3Position.val(),
            switchToScene: scriptSwitchToScene.val()
        };
        $.ajax({
            data: JSON.stringify(newScene),
            contentType: 'application/json; charset=utf-8',
            type: 'POST',
            dataType: 'json',
            url: '/api/script',
            success: function (result) {
                location.reload(true);
            },
            error: function (xhr, status, error) {
                let errorMessage = `${xhr.status} ${status} `;
                if (status !== xhr.statusText) {
                    errorMessage += `(${xhr.statusText}) `;
                }
                errorMessage += `: updating setting ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            },
            complete: hideLoading
        });
    }

    function runAction(act, scene) {
        $.ajax({
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            url: '/api/navigate/' + act + '/' + scene,
            error: function (xhr, status, error) {
                let errorMessage = `${xhr.status} ${status} `;
                if (status !== xhr.statusText) {
                    errorMessage += `(${xhr.statusText}) `;
                }
                errorMessage += `: updating current nave ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            }
        });

        var tempScene = $.grep(fullScript, function (a) {
            return a.act === act && a.scene === scene;
        })[0];
        var obsScene = "";
        switch (tempScene.switchToScene) {
            case "computer":
                obsScene = settingObsComputerScene;
                break;
            case "cam1":
                obsScene = settingObsCam1Scene;
                break;
            case "cam2":
                obsScene = settingObsCam2Scene;
                break;
            case "cam3":
                obsScene = settingObsCam3Scene;
                break;
        }

        if (obsScene !== "") {
            obs.call('SetCurrentProgramScene', { sceneName: obsScene });
        }

        function runActionAPI(camIP, camPosition) {
            $.ajax({
                type: 'GET',
                url: '/api/runAction/' + camIP + '/' + camPosition,
                success: function (result, status, xhr) {
                    console.log("success running camera " + camIP + " position " + camPosition + " action");
                },
                error: function (xhr, status, error) {
                    let errorMessage = `${xhr.status} ${status} `;
                    if (status !== xhr.statusText) {
                        errorMessage += `(${xhr.statusText}) `;
                    }
                    errorMessage += `running camera ${camIP} position ${camPosition} action: ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                    console.error(errorMessage);
                }
            });
        }

        if (tempScene.camera1Action === "prepare") {
            setTimeout(runActionAPI(settingCam1IP, tempScene.camera1Position), 3000);
        }

        if (tempScene.camera2Action === "prepare") {
            setTimeout(runActionAPI(settingCam2IP, tempScene.camera2Position), 3000);
        }

        if (tempScene.camera3Action === "prepare") {
            setTimeout(runActionAPI(settingCam3IP, tempScene.camera3Position), 3000);
        }
    }

    function getScript() {
        showLoading();
        $.ajax({
            type: 'GET',
            url: '/api/script',
            success: function (result, status, xhr) {
                fullScript = result;
                $.each(result, function (index, script) {
                    showLoading();
                    theScript.append('<h2 class="mt-4">Act ' + script.act + '<br />Scene ' + script.scene + '</h2>');
                    var table = '<table class="table table-striped table-bordered table-hover table-sm">';
                    table += '<tr><th>Setting</th><th>Action</th><th>Position</th></tr>';
                    table += '<tr><td>Scene</td><td>' + script.switchToScene + '</td><td></td></tr>';
                    table += '<tr><td>Cam 1</td><td>' + script.camera1Action + '</td><td>' + script.camera1Position + '</td></tr>';
                    table += '<tr><td>Cam 2</td><td>' + script.camera2Action + '</td><td>' + script.camera2Position + '</td></tr>';
                    table += '<tr><td>Cam 3</td><td>' + script.camera3Action + '</td><td>' + script.camera3Position + '</td></tr>';
                    table += '</table>';

                    theScript.append('<div class="row mb-3"><div class="col-md-8 themed-grid-col" style="white-space: pre-line">' + script.text + '</div><div class="col-md-4 themed-grid-col"><button class="btn btn-primary btn-sml" onclick="pageant.runAction(\'' + script.act + '\' , \'' + script.scene + '\')">Run</button><button class="btn btn-success btn-sml" data-bs-toggle="modal" data-bs-target="#script" data-bs-act="' + script.act + '" data-bs-scene="' + script.scene + '">Edit</button><br />' + table + '</div></div>');
                    hideLoading();
                });
            },
            error: function (xhr, status, error) {
                let errorMessage = `${xhr.status} ${status} `;
                if (status !== xhr.statusText) {
                    errorMessage += `(${xhr.statusText}) `;
                }
                errorMessage += `getting scripts: ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            },
            complete: hideLoading
        });
    }

    function getSettings() {
        showLoading();
        $.ajax({
            type: 'GET',
            url: '/api/settings',
            success: function (result, status, xhr) {
                getScript();
                $.each(result, function (index, setting) {
                    showLoading();
                    switch (setting.settingType) {
                        case "OBS":
                            switch (setting.settingId) {
                                case "ComputerScene":
                                    settingObsComputerScene = setting.setting;
                                    break;
                                case "Cam1Scene":
                                    settingObsCam1Scene = setting.setting;
                                    break;
                                case "Cam2Scene":
                                    settingObsCam2Scene = setting.setting;
                                    break;
                                case "Cam3Scene":
                                    settingObsCam3Scene = setting.setting;
                                    break;
                            }
                            break;
                        case "Camera":
                            switch (setting.settingId) {
                                case "1":
                                    settingCam1IP = setting.setting;
                                    break;
                                case "2":
                                    settingCam2IP = setting.setting;
                                    break;
                                case "3":
                                    settingCam3IP = setting.setting;
                                    break;
                            }
                            break;
                    }
                    hideLoading();
                });
            },
            error: function (xhr, status, error) {
                let errorMessage = `${xhr.status} ${status} `;
                if (status !== xhr.statusText) {
                    errorMessage += `(${xhr.statusText}) `;
                }
                errorMessage += `getting settings: ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            },
            complete: hideLoading
        });
    }

    return {
        initialize: initialize,
        runAction: runAction
    }
}());
