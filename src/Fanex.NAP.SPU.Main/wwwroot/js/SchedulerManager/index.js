function SchedulerManager() {
    let domElements = {
        chkActiveJob: $('#chkActiveJob'),
        selEnvironmentDb: $('#selEnvironmentDb'),
        txtJobName: $('#txtJobName'),
        selGroupJob: $('#selGroupJob'),
        selJobType: $('#selJobType'),
        txtApiUrl1: $('#txtApiUrl1'),
        txtApiUrl2: $('#txtApiUrl2'),
        txtApiUrl3: $('#txtApiUrl3'),
        txtApiUrl4: $('#txtApiUrl4'),
        txtApiUrl5: $('#txtApiUrl5'),
        divEnvironmentApiUrl2: $('#divEnvironmentApiUrl2'),
        divEnvironmentApiUrl3: $('#divEnvironmentApiUrl3'),
        divEnvironmentApiUrl4: $('#divEnvironmentApiUrl4'),
        divEnvironmentApiUrl5: $('#divEnvironmentApiUrl5'),
        txtMethodName: $('#txtMethodName'),
        txtController: $('#txtController'),
        rdTriggerType: $('#rdTriggerType'),
        txtCronExpression: $('#txtCronExpression'),
        txtIntervalInSecond: $('#txtIntervalInSecond'),       
        txtTriggerName: $('#txtTriggerName'),
        btnAddJob: $('#btnAddJob'),
        btnSaveJob: $('#btnSaveJob'),
        btnCancelJob: $('#btnCancelJob'),
        btnUpdateJob: $('#btnUpdateJob'),

        chkUpdActiveJob: $('#chkUpdActiveJob'),
        selUpdEnvironmentDb: $('#selUpdEnvironmentDb'),
        txtUpdJobName: $('#txtUpdJobName'),
        selUpdGroupJob: $('#selUpdGroupJob'),
        selUpdJobType: $('#selUpdJobType'),
        txtUpdApiUrl: $('#txtUpdApiUrl'),
        txtUpdMethodName: $('#txtUpdMethodName'),
        txtUpdController: $('#txtUpdController'),
        rdUpdTriggerType: $('#rdUpdTriggerType'),
        txtUpdCronExpression: $('#txtUpdCronExpression'),
        txtUpdIntervalInSecond: $('#txtUpdIntervalInSecond'),
        txtUpdTriggerName: $('#txtUpdTriggerName'),
        btnUpdSaveJob: $('#btnUpdSaveJob'),
        btnUpdCancelJob: $('#btnUpdCancelJob'),

        tabsContainer: $('#tabsContainer'),
        tabsHeader: $('#tabsHeader'),
        tabsContent: $('#tabsContent'),
        btnRestartSchedulerFromDb: $('#btnRestartSchedulerFromDb'),
        btnGetJobsFromDb: $('#btnGetJobsFromDb')    
    }

    const ConstTriggerType = {
        Simple: 'Simple',
        Cron: 'Cron'
    }

    const ConstDbEnvironment = {
        Local: 1
    }

    var objValidate = {
        isError: false,
        data: {
            EnvironmentDbIDs: null,
            EnvironmentApiUrls: null,
            AddJob: null          
        }
    }

    function RestartSchedulerFromDb() {
        if (confirm('Are you sure you want to Restart Scheduler from Db?')) {
            $.ajax({
                url: '/SchedulerManager/RestartSchedulerFromDb',
                type: 'POST',
                success: function (response) {
                    alert("Restart Scheduler from Db successful");
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });
        }       
    }

    function GetJobsFromDb() {
        $.ajax({
            url: '/SchedulerManager/GetJobsFromDb',
            type: 'POST',
            data: {},
            success: function (response) {               
                GenerateTable(response.groupJobs);
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }

    function GetGroupJobs($element, selectedValue) {
        $.ajax({
            url: '/SchedulerManager/GetGroupJobs',
            type: 'POST',
            data: {},
            beforeSend: function () {
                $element.empty();
            },
            success: function (response) {
                response.groups.forEach(row => {
                    $element.append(
                        $('<option>', {
                            value: row.GroupName,
                            text: row.GroupName
                        })
                    )
                })
                if (selectedValue != null && selectedValue != undefined) {
                    $element.val(selectedValue);
                }
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }   

    function AddJob() {
        $(".popup-overlay").fadeIn();

        domElements.txtJobName.val('');      
        domElements.chkActiveJob.prop('checked', false);        
        domElements.selJobType.val($('#selJobType option:first').val());
        domElements.txtApiUrl1.val('');
        domElements.txtApiUrl2.val('');
        domElements.txtApiUrl3.val('');
        domElements.txtApiUrl4.val('');
        domElements.txtApiUrl5.val('');
        domElements.txtMethodName.val('');
        domElements.txtController.val('');  
        domElements.txtCronExpression.val('');
        domElements.txtIntervalInSecond.val('');
        domElements.txtTriggerName.val('');        

        GetGroupJobs(domElements.selGroupJob, null);

        domElements.selEnvironmentDb.chosen({
            placeholder_text_multiple: "Select Environment",
            no_results_text: "No options found!",  
        });

        CreateDOMDependOnEnvironment();
       
        $('option', domElements.selEnvironmentDb).prop('selected', true);

        domElements.selEnvironmentDb.trigger("chosen:updated");       

        domElements.btnCancelJob.off('click').on('click', function () { $(".popup-overlay").fadeOut(); });

        domElements.btnSaveJob.off('click').on('click', function () { SaveAddJob(); })
    }  

    function CreateDOMDependOnEnvironment() {
        if (dbEnvironment == ConstDbEnvironment.Local) {
            domElements.selEnvironmentDb.append(`
                <option value="1">Environment 1</option>
                <option value="2">Environment 2</option>
                <option value="3">Environment 3</option>
                <option value="4">Environment 4</option>
                <option value="5">Environment 5</option>
            `);
        } else {
            domElements.selEnvironmentDb.append(`<option value="1">Environment 1</option>`);
        }                

        domElements.divEnvironmentApiUrl2.prop('hidden', true);
        domElements.divEnvironmentApiUrl3.prop('hidden', true);
        domElements.divEnvironmentApiUrl4.prop('hidden', true);
        domElements.divEnvironmentApiUrl5.prop('hidden', true);
    }
    
    function SaveAddJob() {
        var validateResult = ValidateDataJob(false);

        if (validateResult.isError) {
            alert('Input field * is required');

            return;
        }

        if (confirm('Are you sure you want to add this job?')) {
            $.post("/SchedulerManager/CheckExistJob", {
                jobName: domElements.txtJobName.val().trim(),              
                dbEnvironmentIDs: domElements.selEnvironmentDb.val().join()
            }, function (response) {
                if (response != undefined && response.jobs != undefined && response.jobs.length > 0) {
                    alert('This job Name and Group already exists');

                    console.log(response.jobs);
                } else {
                    $.ajax({
                        url: '/SchedulerManager/AddJobToDb',
                        type: 'POST',
                        data: validateResult.data,
                        success: function (response) {
                            alert('Save Add Job to db successful');

                            $(".popup-overlay").fadeOut();

                            GetJobsFromDb();
                        },
                        error: function (xhr, status, error) {
                            alert('Error: ' + error);
                        }
                    });         
                }
            });             
        }
    }    

    function GetDataUpdateJob(isUpdate) {
        var objResult = new Object();
        let environmentApiUrls = [];

        if (isUpdate) {
            environmentApiUrls.push(domElements.txtUpdApiUrl.val());           

            objResult.data = {
                EnvironmentDbIDs: domElements.selUpdEnvironmentDb.val(),
                EnvironmentApiUrls: environmentApiUrls,
                AddJob: {
                    JobName: domElements.txtUpdJobName.val().trim(),
                    GroupName: domElements.selUpdGroupJob.val(),
                    IsStartByDefault: domElements.chkUpdActiveJob.prop('checked'),
                    IsAdjust: true,
                    JobType: domElements.selUpdJobType.val(),                  
                    MethodName: domElements.txtUpdMethodName.val().trim(),
                    Controller: domElements.txtUpdController.val().trim(),
                    TriggerType: $("input[name='rdUpdTriggerType']:checked").val(),
                    CronExpression: domElements.txtUpdCronExpression.val().trim(),
                    IntervalInSecond: domElements.txtUpdIntervalInSecond.val().trim(),
                    TriggerName: domElements.txtUpdTriggerName.val().trim()
                }                
            };
        } else {
            environmentApiUrls.push(domElements.txtApiUrl1.val());
            environmentApiUrls.push(domElements.txtApiUrl2.val());
            environmentApiUrls.push(domElements.txtApiUrl3.val());
            environmentApiUrls.push(domElements.txtApiUrl4.val());
            environmentApiUrls.push(domElements.txtApiUrl5.val());

            objResult.data = {                
                EnvironmentDbIDs: domElements.selEnvironmentDb.val().join(),
                EnvironmentApiUrls: environmentApiUrls,
                AddJob: {
                    JobName: domElements.txtJobName.val().trim(),
                    GroupName: domElements.selGroupJob.val(),
                    IsStartByDefault: domElements.chkActiveJob.prop('checked'),
                    IsAdjust: true,
                    JobType: domElements.selJobType.val(),                  
                    MethodName: domElements.txtMethodName.val().trim(),
                    Controller: domElements.txtController.val().trim(),
                    TriggerType: $("input[name='rdTriggerType']:checked").val(),
                    CronExpression: domElements.txtCronExpression.val().trim(),
                    IntervalInSecond: domElements.txtIntervalInSecond.val().trim(),
                    TriggerName: domElements.txtTriggerName.val().trim()
                }                
            };
        }
        
        return objResult;
    }

    function ValidateDataJob(isUpdate) {               
        var objResult = GetDataUpdateJob(isUpdate);
        objResult.isError = false;

        if (objResult.data.JobName == '') {
            objResult.isError = true;
        }
        if (objResult.data.GroupName == '') {
            objResult.isError = true;
        }
        
        if (objResult.data.MethodName == '') {
            objResult.isError = true;
        }
        if (objResult.data.Controller == '') {
            objResult.isError = true;
        }
        if (objResult.data.TriggerName == '') {
            objResult.isError = true;
        }
        if (objResult.data.CronExpression == '' && objResult.data.IntervalInSecond == '') {
            objResult.isError = true;
        }
        let valEnvironmentDb = isUpdate ? domElements.selUpdEnvironmentDb.val() : domElements.selEnvironmentDb.val();
        $.each(valEnvironmentDb, function (idx, value) {
            if (objResult.data.EnvironmentApiUrls[parseInt(value) - 1] == '') {
                objResult.isError = true;
            }
        })

        return objResult;
    }

    function UpdateJob(jobName, groupName)
    {
        $(".upd-popup-overlay").fadeIn();

        domElements.txtUpdJobName.val('');
        domElements.chkUpdActiveJob.prop('checked', false);
        domElements.selUpdJobType.val($('#selUpdJobType option:first').val());
        domElements.txtUpdApiUrl.val('');
        domElements.txtUpdMethodName.val('');
        domElements.txtUpdController.val('');
        domElements.txtUpdCronExpression.val('');
        domElements.txtUpdIntervalInSecond.val('');
        domElements.txtUpdTriggerName.val('');
        domElements.btnUpdSaveJob.prop('disabled', true);

        GetGroupJobs(domElements.selUpdGroupJob, groupName);

        domElements.selUpdEnvironmentDb.chosen({
            placeholder_text_multiple: "Select Environment",
            no_results_text: "No options found!",
        });
      
        domElements.selUpdEnvironmentDb.prop("disabled", true);
        domElements.selUpdEnvironmentDb.trigger("chosen:updated");

        domElements.btnUpdCancelJob.off('click').on('click', function () { $(".upd-popup-overlay").fadeOut(); });

        domElements.btnUpdSaveJob.off('click').on('click', function () { SaveUpdateJob(); });

        domElements.txtUpdJobName.off('keypress').on('keypress', function (e) {          
            if (e && e.which == 13) { // enter            
                GetJobUpdate(null, null);
            }
        });

        domElements.selUpdGroupJob.off('change').on('change', function () { GetJobUpdate(null, null); });

        if (jobName != '' && groupName != ''
            && jobName != undefined && groupName != undefined) {   
            domElements.txtUpdJobName.val(jobName);
            domElements.txtUpdJobName.prop("readonly", true);
            domElements.selUpdGroupJob.prop("disabled", true);
            domElements.txtUpdTriggerName.prop("readonly", true);

            GetJobUpdate(jobName, groupName);
        } else {
            domElements.txtUpdJobName.prop("readonly", false);
            domElements.selUpdGroupJob.prop("disabled", false);
            domElements.txtUpdTriggerName.prop("readonly", false);
        }
    }

    function GetJobUpdate(jobName, groupName) {
        if (jobName == null || groupName == null) {
            jobName = domElements.txtUpdJobName.val().trim();
            groupName = domElements.selUpdGroupJob.val();
        }

        $.ajax({
            url: '/SchedulerManager/GetSingleJob',
            type: 'POST',
            data: {
                jobName: jobName,
                groupName: groupName
            },
            success: function (response) {
                if (response != undefined && response.jobTrigger != undefined) {
                    domElements.chkUpdActiveJob.prop('checked', response.jobTrigger.IsStartByDefault); 
                    domElements.selUpdEnvironmentDb.val(response.jobTrigger.EnvironmentID);
                    domElements.selUpdEnvironmentDb.trigger('chosen:updated');
                    domElements.selUpdJobType.val(response.jobTrigger.JobType);
                    domElements.txtUpdApiUrl.val(response.jobTrigger.APIUrl);
                    domElements.txtUpdMethodName.val(response.jobTrigger.MethodName);
                    domElements.txtUpdController.val(response.jobTrigger.Controller);
                    domElements.txtUpdTriggerName.val(response.jobTrigger.Triggers[0].TriggerName);
                    domElements.txtUpdCronExpression.val(response.jobTrigger.Triggers[0].CronExpression);
                    domElements.txtUpdIntervalInSecond.val(response.jobTrigger.Triggers[0].IntervalInSecond);
                    if (response.jobTrigger.Triggers[0].TriggerType == ConstTriggerType.Simple) {
                        $(`input[name="rdUpdTriggerType"][value='${ConstTriggerType.Simple}']`).prop('checked', true);
                    } else {
                        $(`input[name="rdUpdTriggerType"][value='${ConstTriggerType.Cron}']`).prop('checked', true);   
                    }
                    domElements.btnUpdSaveJob.prop('disabled', false);
                } else {                 
                    domElements.chkUpdActiveJob.prop('checked', false);
                    domElements.selUpdJobType.val($('#selUpdJobType option:first').val());
                    domElements.txtUpdApiUrl.val('');
                    domElements.txtUpdMethodName.val('');
                    domElements.txtUpdController.val('');
                    domElements.txtUpdCronExpression.val('');
                    domElements.txtUpdIntervalInSecond.val('');
                    domElements.txtUpdTriggerName.val('');
                    domElements.btnUpdSaveJob.prop('disabled', true);
                }
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }

    function SaveUpdateJob()
    {
        var validateResult = ValidateDataJob(true);

        if (validateResult.isError) {
            alert('Input field * is required');

            return;
        }

        if (confirm('Are you sure you want to update this job?')) {
            $.ajax({
                url: '/SchedulerManager/UpdateJobToDb',
                type: 'POST',
                data: validateResult.data,
                success: function (response) {
                    alert('Save Update Job successful');

                    $(".upd-popup-overlay").fadeOut();

                    GetJobsFromDb();
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });
        }
    }

    function ApplyAdjustJobFromDb(jobName, groupName) {
        if (confirm('Are you sure you want to apply this job?')) {
            $.ajax({
                url: '/SchedulerManager/ApplyAdjustJobFromDb',
                type: 'POST',
                data: {
                    jobName: jobName,
                    groupName: groupName
                },
                success: function (response) {
                    alert(`Apply Ajust Job ${jobName} successful`);

                    GetJobsFromDb();
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });
        }        
    }

    function ReloadJobFromDb(jobName, groupName) {     
        if (confirm('Are you sure you want to update this job?')) {
            $.ajax({
                url: '/SchedulerManager/ReloadJobFromDb',
                type: 'POST',
                data: {
                    jobName: jobName,
                    groupName: groupName
                },
                success: function (response) {
                    alert(`Update Job ${jobName} successful`);
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });
        }        
    }

    function DeleteJobToDb(jobName, groupName) {     
        if (confirm('Are you sure you want to delete this job?')) {
            $.ajax({
                url: '/SchedulerManager/DeleteJobToDb',
                type: 'POST',
                data: {
                    jobName: jobName,
                    groupName: groupName
                },
                success: function (response) {
                    alert(`Delete Job ${jobName} successful`);

                    GetJobsFromDb();
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });
        }       
    }
  
    function GenerateTable(data) {
        domElements.tabsHeader.empty();
        domElements.tabsContent.empty();

        if (data != undefined) {
            let active = 'active';
            let i = 0;
            data.forEach(group => {
                domElements.tabsHeader.append(`<div class='tab ${active}' data-tab="${group.Name}">${group.Name}</div>`);

                if (group.Jobs != undefined) {
                    let $div = $(`<div class='tab-content ${active}' id='tab-${group.Name}'></div>`);
                    active = '';

                    let $table = InitTable(group.Name);

                    $div.append($table);

                    domElements.tabsContent.append($div);

                    RenderTable(group.Jobs, group.Name, $table);
                }     
            })         
        }        

        WriteTabEvent();
    }

    function InitTable(groupName) {       
        let $table = $(`<table id="${groupName}" class="table table-striped"></table>`);
        let $thead = $(`<thead><tr>
                            <th></th>
                            <th>JobName</th>                          
                            <th>APIUrl</th>
                            <th>Method</th>                            
                            <th>Has Adjust</th>
                            <th>Action</th>
                            </tr></thead>`);
        $table.append($thead);

        let $tbody = $('<tbody></tbody>');

        $table.append($tbody);

        return $table;
    }

    function RenderTable(data, groupName, $table) {
        $table.DataTable({
            dom: '<"wrapper-info-table"<"wrapper-info"lip>>t',
            paging: false,
            pagingType: "simple",
            language: {
                lengthMenu: "Page size _MENU_",
                paginate: {
                    first: "",
                    last: "",
                    next: "",
                    previous: ""
                },
                info: "Displaying _START_ to _END_ of _TOTAL_ item(s)",
                infoEmpty: "",
                zeroRecords: "There's no data"
            },
            searching: true,
            scrollX: true,
            scrollCollapse: true,
            order: [[0, "asc"]],
            columnDefs: [
                { targets: "_all", defaultContent: "-" },
                { targets: [0, 1, 2, 3, 4, 5], className: "text-left" },
                { targets: [0], width: "5%" },
                { targets: [1], width: "20%" },
                { targets: [2], width: "20%" },
                { targets: [3], width: "15%" },
                { targets: [4], width: "15%" },
                { targets: [5], width: "15%" }
            ],
            data: data,            
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; },
                },
                { data: obj => obj.JobName },
                { data: obj => obj.APIUrl },
                { data: obj => () => { return obj.Controller + "/" + obj.MethodName } },             
                {
                    data: obj => () => { 
                        if (obj.IsAdjust) {
                            return `<span class='btnApplyAdjustJobFromDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Apply Scheduler from Db'><i class='icon-approval'></i></span>`;
                        } else {
                            return '';
                        }
                        
                    }, createdCell: (cell, _, row) => {
                        $('.btnApplyAdjustJobFromDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');

                            ApplyAdjustJobFromDb(job, group);
                        })
                    }
                },
                {
                    data: obj => () => {
                        let btnReloadJobFromDb = `<span class='btnReloadJobFromDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Reload job from Db'><i class='icon-autorenew'></i></span>`;

                        let btnEditJobFromDb = `<span class='btnEditJobFromDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Edit job from Db'><i class='icon-edit'></i></span>`;

                        let btnDeleteJobToDb = `<span class='btnDeleteJobToDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Remove job to Db'><i class='icon-delete'></i></span>`;
                        return html = btnReloadJobFromDb + btnEditJobFromDb + btnDeleteJobToDb;
                    }, createdCell: (cell, _, row) => {
                        $('.btnReloadJobFromDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');

                            ReloadJobFromDb(job, group);
                        })

                        $('.btnEditJobFromDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');

                            UpdateJob(job, group);
                        })

                        $('.btnDeleteJobToDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');
                          
                            DeleteJobToDb(job, group);
                        })
                    }
                }
            ]
        });

        return $table;
    }

    function WriteTabEvent() {
        $('.tab').on('click', function () {          
            $('.tab').removeClass('active');
            $('.tab-content').removeClass('active');
          
            $(this).addClass('active');
        
            const target = $(this).data('tab');
           
            $(`#tab-${target}`).addClass('active');
        });         
    }
   
    function Initial() {        
      
        domElements.btnAddJob.on('click', function () { AddJob(); });

        domElements.btnUpdateJob.on('click', function () { UpdateJob(); });
       
        domElements.btnRestartSchedulerFromDb.on('click', function () { RestartSchedulerFromDb(); });

        domElements.btnGetJobsFromDb.on('click', function () { GetJobsFromDb(); });
               
    }

    $(document).ready(function () {
        Initial();
    })
}

$(SchedulerManager);