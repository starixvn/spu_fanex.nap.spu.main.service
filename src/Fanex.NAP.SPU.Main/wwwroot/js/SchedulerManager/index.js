function SchedulerManager() {
    let domElements = {
        chkActiveJob: $('#chkActiveJob'),
        txtJobName: $('#txtJobName'),
        selGroupJob: $('#selGroupJob'),
        selJobType: $('#selJobType'),
        txtApiUrl: $('#txtApiUrl'),
        txtMethodName: $('#txtMethodName'),
        txtController: $('#txtController'),
        rdTriggerType: $('#rdTriggerType'),
        txtCronExpression: $('#txtCronExpression'),
        txtInterval: $('#txtInterval'),       
        txtTriggerName: $('#txtTriggerName'),
        btnAddJob: $('#btnAddJob'),
        btnSaveJob: $('#btnSaveJob'),
        btnCancelJob: $('#btnCancelJob'),
        tabsContainer: $('#tabsContainer'),
        tabsHeader: $('#tabsHeader'),
        tabsContent: $('#tabsContent'),
        btnRestartSchedulerFromDb: $('#btnRestartSchedulerFromDb'),
        btnGetJobsFromDb: $('#btnGetJobsFromDb'),
        btnUpdateJobsFromDb: $('#btnUpdateJobsFromDb'),
        txtUpdateJobName: $('#txtUpdateJobName'),
        txtUpdateGroupName: $('#txtUpdateGroupName'),
        btnUpdateJob: $('#btnUpdateJob'),
        txtDeleteJobName: $('#txtDeleteJobName'),
        txtDeleteGroupName: $('#txtDeleteGroupName'),
        btnDeleteJob: $('#btnDeleteJob'),        
        btnDeleteAllJobs: $('#btnDeleteAllJobs')      
    }

    function RestartSchedulerFromDb() {
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

    function GetGroupJobs() {
        $.ajax({
            url: '/SchedulerManager/GetGroupJobs',
            type: 'POST',
            data: {},
            beforeSend: function () {
                domElements.selGroupJob.empty();
            },
            success: function (response) {
                response.groups.forEach(row => {
                    domElements.selGroupJob.append(
                        $('<option>', {
                            value: row.GroupName,
                            text: row.GroupName
                        })
                    )
                })
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }

    function UpdateJobsFromDb() {
        $.ajax({
            url: '/SchedulerManager/UpdateJobsFromDb',
            type: 'POST',
            data: { },
            success: function (response) {                
                alert(`Update Jobs from Db successful`);
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }

    function AddJob() {
        $(".popup-overlay").fadeIn();

        GetGroupJobs();

        domElements.btnCancelJob.on('click', function () { $(".popup-overlay").fadeOut(); });
       
        domElements.btnSaveJob.on('click', function () { SaveAddJob(); })
    }  
    
    function SaveAddJob() {
        if (confirm('Are you sure you want to add this job?')) {
            $.ajax({
                url: '/SchedulerManager/AddJob',
                type: 'POST',
                data: {
                    JobName: domElements.txtJobName.val(),
                    GroupName: domElements.selGroupJob.val(),
                    IsActvie: domElements.chkActiveJob.prop('checked'),
                    IsNew: true,
                    JobType: domElements.selJobType.val(),
                    ApiUrl: domElements.txtApiUrl.val(),
                    MethodName: domElements.txtMethodName.val(),
                    Controller: domElements.txtController.val(),
                    TriggerType: $("input[name='rdTriggerType']:checked").val(),
                    CronExpression: domElements.txtCronExpression.val(),
                    Interval: domElements.txtInterval.val(),                   
                    TriggerName: domElements.txtTriggerName.val()
                },
                success: function (response) {
                    alert('Save Add Job successful');

                    $(".popup-overlay").fadeOut();
                },
                error: function (xhr, status, error) {
                    alert('Error: ' + error);
                }
            });           
        }
    }

    function ApplyNewJobFromDb(jobName, groupName) {
        $.ajax({
            url: '/SchedulerManager/ApplyNewJobFromDb',
            type: 'POST',
            data: {
                jobName: jobName,
                groupName: groupName
            },
            success: function (response) {
                alert(`Apply New Job ${jobName} successful`);
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }

    function ReloadJobFromDb(jobName, groupName) {       
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

    function DeleteJobToDb(jobName, groupName) {       
        $.ajax({
            url: '/SchedulerManager/DeleteJobToDb',
            type: 'POST',
            data: {
                jobName: jobName,
                groupName: groupName
            },
            success: function (response) {               
                alert(`Delete Job ${jobName} successful`);
            },
            error: function (xhr, status, error) {               
                alert('Error: ' + error);
            }
        });
    }

    function DeleteAllJob() {
        $.ajax({
            url: '/SchedulerManager/DeleteAllJobs',
            type: 'POST',
            success: function (response) {
                alert("Delete All Jobs successful");
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
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
                            <th>MethodName</th>
                            <th>Controller</th>
                            <th>New</th>
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
                { targets: [0, 1, 2, 3, 4, 5, 6], className: "text-left" },
                { targets: [0], width: "2%" },
                { targets: [1], width: "9%" },
                { targets: [2], width: "9%" },
                { targets: [3], width: "9%" },
                { targets: [4], width: "9%" },
                { targets: [5], width: "9%" },
                { targets: [6], width: "9%" }
            ],
            data: data,            
            columns: [
                {
                    render: function (data, type, full, meta) { return meta.row + 1; },
                },
                { data: obj => obj.JobName },
                { data: obj => obj.APIUrl },
                { data: obj => obj.MethodName },
                { data: obj => obj.Controller },
                {
                    data: obj => () => { 
                        if (obj.IsNew) {
                            return `<span class='btnApplyNewJobFromDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Apply Scheduler from Db'><i class='icon-approval'></i></span>`;
                        } else {
                            return '';
                        }
                        
                    }, createdCell: (cell, _, row) => {
                        $('.btnApplyNewJobFromDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');

                            ApplyNewJobFromDb(job, group);
                        })
                    }
                },
                {
                    data: obj => () => {
                        let btnReloadJobFromDb = `<span class='btnReloadJobFromDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Reload Scheduler from Db'><i class='icon-autorenew'></i></span>`;

                        let btnDeleteJobToDb = `<span class='btnDeleteJobToDb sub-icon' data-group='${groupName}' data-job='${obj.JobName}'
                                            title='Remove Scheduler to Db'><i class='icon-delete'></i></span>`;
                        return html = btnReloadJobFromDb + btnDeleteJobToDb;
                    }, createdCell: (cell, _, row) => {                       
                        $('.btnReloadJobFromDb', cell).on('click', function () {
                            let group = $(this).attr('data-group');
                            let job = $(this).attr('data-job');

                            ReloadJobFromDb(job, group);
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
      
        domElements.btnAddJob.on('click', function () { AddJob(); })
       
        domElements.btnRestartSchedulerFromDb.on('click', function () { RestartSchedulerFromDb(); });

        domElements.btnGetJobsFromDb.on('click', function () { GetJobsFromDb(); })

        domElements.btnUpdateJobsFromDb.on('click', function () { UpdateJobsFromDb(); });        

        domElements.btnUpdateJob.on('click', function () { UpdateJob(); });

        domElements.btnDeleteJob.on('click', function () { DeleteJob(); });

        domElements.btnDeleteAllJobs.on('click', function () { DeleteAllJob(); });
     
    }

    $(document).ready(function () {
        Initial();
    })
}

$(SchedulerManager);