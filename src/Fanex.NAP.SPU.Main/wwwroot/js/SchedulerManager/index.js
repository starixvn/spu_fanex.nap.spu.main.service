function SchedulerManager() {
    let domElements = {
        btnRestartSchedulerFromDb: $('#btnRestartSchedulerFromDb'),
        btnLoadJobsUpdateFromDb: $('#btnLoadJobsUpdateFromDb'),
        btnUpdateJobsFromDb: $('#btnUpdateJobsFromDb'),
        txtUpdateJobName: $('#txtUpdateJobName'),
        txtUpdateGroupName: $('#txtUpdateGroupName'),
        btnUpdateJob: $('#btnUpdateJob'),
        txtDeleteJobName: $('#txtDeleteJobName'),
        txtDeleteGroupName: $('#txtDeleteGroupName'),
        btnDeleteJob: $('#btnDeleteJob'),        
        btnDeleteAllJobs: $('#btnDeleteAllJobs'),
        btnUpdateFlagJobUpdate: $('#btnUpdateFlagJobUpdate')
        
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

    function LoadJobsUpdateFromDb() {
        $.ajax({
            url: '/SchedulerManager/LoadJobsUpdateFromDb',
            type: 'POST',
            data: {},
            success: function (response) {
                console.log(response);
                alert(`Load Jobs successful`);
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

    function UpdateJob() {
        let jobName = domElements.txtUpdateJobName.val();
        let groupName = domElements.txtUpdateGroupName.val();

        $.ajax({
            url: '/SchedulerManager/UpdateJob',
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

    function DeleteJob() {
        let jobName = domElements.txtDeleteJobName.val();
        let groupName = domElements.txtDeleteGroupName.val();

        $.ajax({
            url: '/SchedulerManager/DeleteJob',
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

    function UpdateFlagJobUpdate() {
        $.ajax({
            url: '/SchedulerManager/UpdateFlagJobUpdate',
            type: 'POST',
            success: function (response) {
                alert("Update Flag Job Update successful");
            },
            error: function (xhr, status, error) {
                alert('Error: ' + error);
            }
        });
    }  

    function Initial() {
        domElements.btnRestartSchedulerFromDb.on('click', function () { RestartSchedulerFromDb(); });

        domElements.btnLoadJobsUpdateFromDb.on('click', function () { LoadJobsUpdateFromDb(); })

        domElements.btnUpdateJobsFromDb.on('click', function () { UpdateJobsFromDb(); });        

        domElements.btnUpdateJob.on('click', function () { UpdateJob(); });

        domElements.btnDeleteJob.on('click', function () { DeleteJob(); });

        domElements.btnDeleteAllJobs.on('click', function () { DeleteAllJob(); });

        domElements.btnUpdateFlagJobUpdate.on('click', function () { UpdateFlagJobUpdate(); });
    }

    $(document).ready(function () {
        Initial();
    })
}

$(SchedulerManager);