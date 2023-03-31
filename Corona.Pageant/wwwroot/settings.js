var settings = (function () {
    var obsScenes = [];
    var loading = [];
    var settingsLoading = null;
    var settingsMain = null;
    var functionKey = "";
    var settingObsComputerScene = "";
    var settingObsCam1Scene = "";
    var settingObsCam2Scene = "";
    var settingObsCam3Scene = "";
    var dropdownObsComputerScene = null;
    var dropdownObsCam1Scene = null;
    var dropdownObsCam2Scene = null;
    var dropdownObsCam3Scene = null;
    var textCam1ip = null;
    var textCam2ip = null;
    var textCam3ip = null;
    var ipCam1Save = null;
    var ipCam2Save = null;
    var ipCam3Save = null;
    var exportPrepare = null;
    var exportDiv = null;
    var importFile = null;

    const obs = new OBSWebSocket();
    obs.on('ConnectionOpened', () => {
        showLoading();
        obs.send('GetSceneList').then(data => {
            $.each(data.scenes, function (index, scene) {
                obsScenes.push(scene.name);
            });
            updateObsDropdowns();
            hideLoading();
        })
    });

    const initialize = function () {
        settingsLoading = $("#settingsLoading");
        settingsMain = $("#settingsMain");
        dropdownObsComputerScene = $("#obsComputer");
        dropdownObsComputerScene.change({ scene: "ComputerScene" }, changeObsSetting);
        dropdownObsCam1Scene = $("#obsCam1");
        dropdownObsCam1Scene.change({ scene: "Cam1Scene" }, changeObsSetting);
        dropdownObsCam2Scene = $("#obsCam2");
        dropdownObsCam2Scene.change({ scene: "Cam2Scene" }, changeObsSetting);
        dropdownObsCam3Scene = $("#obsCam3");
        dropdownObsCam3Scene.change({ scene: "Cam3Scene" }, changeObsSetting);
        buttonObsComputer = $("#obsComputerTest");
        buttonObsComputer.click({ scene: "ComputerScene" }, showObsScene);
        buttonObsCam1 = $("#obsCam1Test");
        buttonObsCam1.click({ scene: "Cam1Scene" }, showObsScene);
        buttonObsCam2 = $("#obsCam2Test");
        buttonObsCam2.click({ scene: "Cam2Scene" }, showObsScene);
        buttonObsCam3 = $("#obsCam3Test");
        buttonObsCam3.click({ scene: "Cam3Scene" }, showObsScene);
        textCam1ip = $("#ipCam1");
        textCam2ip = $("#ipCam2");
        textCam3ip = $("#ipCam3");
        ipCam1Save = $("#ipCam1Save");
        ipCam1Save.click({ camera: "1" }, saveCameraIp);
        ipCam2Save = $("#ipCam2Save");
        ipCam2Save.click({ camera: "2" }, saveCameraIp);
        ipCam3Save = $("#ipCam3Save");
        ipCam3Save.click({ camera: "3" }, saveCameraIp);
        exportPrepare = $("#prepareExport");
        exportPrepare.click(prepareExport);
        exportDiv = $("#export");
        importFile = $("#importFile");
        importFile.on('change', fileUploaded);

        // addCategoryButton.click(addCategory);
        getSettings();
        obs.connect({ address: "localhost:4444" });
    };

    function fileUploaded() {
        // TODO: Upload File from importFile
        debugger;
        var formData = new FormData();
        formData.append('file', importFile[0].files[0]);
        $.ajax({
               url : '/api/import/file',
               type : 'POST',
               data : formData,
               processData: false,  // tell jQuery not to process the data
               contentType: false,  // tell jQuery not to set contentType
               success : function() {
                   debugger;
                   importFile.val('');
               }
            });
    }

    function prepareExport() {
        exportPrepare.hide();
        showLoading();
        $.ajax({
            type: 'GET',
            url: '/api/export',
            success: function (result, status, xhr) {
                showLoading();
                var data = "text/json;charset=utf-8," + encodeURIComponent(JSON.stringify(result, undefined, 2));
                var a = document.createElement('a');
                a.href = 'data:' + data;
                a.download = 'export.json';
                a.innerHTML = 'Download Export';
                a.classList.add('btn');
                a.classList.add('btn-warning');
                a.addEventListener('click', () => {
                    setTimeout(() => {
                        exportPrepare.show();
                        exportDiv.empty();
                    }, 250);
                });
                exportDiv.append(a);
                hideLoading();
            },
            error: function (xhr, status, error) {
                let errorMessage = `${xhr.status} ${status} `;
                if (status !== xhr.statusText) {
                    errorMessage += `(${xhr.statusText}) `;
                }
                errorMessage += `getting exports: ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            },
            complete: function () {
                hideLoading();
            }
        });

    }

    function saveCameraIp(camera) {
        showLoading();
        var newValue = "";
        switch (camera.data.camera) {
            case "1":
                newValue = textCam1ip.val();
                break;
            case "2":
                newValue = textCam2ip.val();
                break;
            case "3":
                newValue = textCam3ip.val();
                break;
        }

        if (newValue !== "") {
            $.ajax({
                data: { ipAddress: newValue },
                type: 'POST',
                url: '/api/camera/' + camera.data.camera + '?code=' + functionKey,
                success: function (result) { },
                error: function (xhr, status, error) {
                    let errorMessage = `${xhr.status} ${status} `;
                    if (status !== xhr.statusText) {
                        errorMessage += `(${xhr.statusText}) `;
                    }
                    errorMessage += `: updating setting ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                    console.error('error', 'toast-top-right', errorMessage);
                },
                complete: hideLoading
            });
        }
    }

    function changeObsSetting(scene) {
        showLoading();
        var newValue = "";
        switch (scene.data.scene) {
            case "ComputerScene":
                newValue = dropdownObsComputerScene.val();
                break;
            case "Cam1Scene":
                newValue = dropdownObsCam1Scene.val();
                break;
            case "Cam2Scene":
                newValue = dropdownObsCam2Scene.val();
                break;
            case "Cam3Scene":
                newValue = dropdownObsCam3Scene.val();
                break;
        }

        if (newValue !== "") {
            $.ajax({
                data: { scene: newValue },
                type: 'POST',
                url: '/api/obs/' + scene.data.scene + '?code=' + functionKey,
                success: function (result) {
                    switch (scene.data.scene) {
                        case "ComputerScene":
                            settingObsComputerScene = newValue;
                            break;
                        case "Cam1Scene":
                            settingObsCam1Scene = newValue;
                            break;
                        case "Cam2Scene":
                            settingObsCam2Scene = newValue;
                            break;
                        case "Cam3Scene":
                            settingObsCam3Scene = newValue;
                            break;
                    }
                },
                error: function (xhr, status, error) {
                    let errorMessage = `${xhr.status} ${status} `;
                    if (status !== xhr.statusText) {
                        errorMessage += `(${xhr.statusText}) `;
                    }
                    errorMessage += `: updating setting ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                    console.error('error', 'toast-top-right', errorMessage);
                },
                complete: hideLoading
            });
        }
    }

    function showLoading() {
        loading.push(1);
        settingsLoading.show();
        settingsMain.hide();
    }

    function hideLoading() {
        loading.pop();
        if (loading.length === 0) {
            settingsLoading.hide();
            settingsMain.show();
        }
    }

    function showObsScene(sceneName) {
        var scene = "";
        switch (sceneName.data.scene) {
            case "ComputerScene":
                scene = settingObsComputerScene;
                break;
            case "Cam1Scene":
                scene = settingObsCam1Scene;
                break;
            case "Cam2Scene":
                scene = settingObsCam2Scene;
                break;
            case "Cam3Scene":
                scene = settingObsCam3Scene;
                break;
        }

        if (scene !== "") {
            obs.send('SetCurrentScene', { 'scene-name': scene }).then(data => { console.log(data); });
        }
    }

    function getSettings() {
        showLoading();
        $.ajax({
            type: 'GET',
            url: '/api/settings',
            success: function (result, status, xhr) {
                $.each(result, function (index, setting) {
                    showLoading();
                    switch (setting.settingType) {
                        case "Function":
                            functionKey = setting.setting;
                            break;
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
                                    textCam1ip.val(setting.setting);
                                    break;
                                case "2":
                                    textCam2ip.val(setting.setting);
                                    break;
                                case "3":
                                    textCam3ip.val(setting.setting);
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
                errorMessage += `getting intents for category ${selectedCategory}: ${xhr.responseJSON ? xhr.responseJSON.message : xhr.responseText}. Please try again later`;
                console.error(errorMessage);
            },
            complete: function () {
                hideLoading();
                updateObsDropdowns();
            }
        });
    }

    function updateObsDropdowns() {
        dropdownObsComputerScene.empty();
        dropdownObsCam1Scene.empty();
        dropdownObsCam2Scene.empty();
        dropdownObsCam3Scene.empty();
        dropdownObsComputerScene.append(`<option value="">Select a scene</option>`);
        dropdownObsCam1Scene.append(`<option value="">Select a scene</option>`);
        dropdownObsCam2Scene.append(`<option value="">Select a scene</option>`);
        dropdownObsCam3Scene.append(`<option value="">Select a scene</option>`);
        $.each(obsScenes, function (index, scene) {
            if (scene === settingObsComputerScene) {
                dropdownObsComputerScene.append(`<option value="${scene}" selected>${scene}</option>`);
            } else {
                dropdownObsComputerScene.append(`<option value="${scene}">${scene}</option>`);
            }

            if (scene === settingObsCam1Scene) {
                dropdownObsCam1Scene.append(`<option value="${scene}" selected>${scene}</option>`);
            } else {
                dropdownObsCam1Scene.append(`<option value="${scene}">${scene}</option>`);
            }

            if (scene === settingObsCam2Scene) {
                dropdownObsCam2Scene.append(`<option value="${scene}" selected>${scene}</option>`);
            } else {
                dropdownObsCam2Scene.append(`<option value="${scene}">${scene}</option>`);
            }

            if (scene === settingObsCam3Scene) {
                dropdownObsCam3Scene.append(`<option value="${scene}" selected>${scene}</option>`);
            }
            else {
                dropdownObsCam3Scene.append(`<option value="${scene}">${scene}</option>`);
            }
        });
    }

    return {
        initialize: initialize
    }
}());
