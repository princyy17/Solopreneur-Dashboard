
    $(document).ready(function () {
        $("#openModal").click(function (e) {
            e.preventDefault();
            console.log("exada");
            $.get('/Project/Add', function (data) {
                console.log(data);
                $("#modal-container").html(data);
                $("#modal").removeClass("hidden");

                $('body').css('overflow', 'hidden');
            }).fail(function (xhr) {
                console.error("Error Loading Modal:", xhr.responseText);
            });
        });

    $(".dropdown-btn").click(function (event) {
        event.stopPropagation();
    $(".dropdown-menu").not($(this).next()).hide();
    $(this).next(".dropdown-menu").toggle();
        });

    $(document).click(function () {
        $(".dropdown-menu").hide();
        });

    $(".dropdown-menu a").click(function (event) {
        event.preventDefault();
    var statusFilter = $(this).text();
    updateProjectsList(statusFilter);
        });
    });
    function Edit(projectId) {
        $.get('/Project/Edit/' + projectId, function (data) {
            console.log(data);
            $("#modal-container").html(data);
            $("#modal").removeClass("hidden");
            $('body').css('overflow', 'hidden');
        }).fail(function (xhr) {
            console.error("Error Loading Project Data:", xhr.responseText);
        });
    }

    let projectIdToDelete = null;

    function openDeleteModal(projectId) {
        projectIdToDelete = projectId;
    document.getElementById('projectId').value = projectId;
    document.getElementById('deleteModal').classList.remove('hidden');
    }

    function closeDeleteModal() {
        document.getElementById('deleteModal').classList.add('hidden');
    }

    function updateProjectsList(statusFilter) {
        $.ajax({
            url: '@Url.Action("Index", "Project")',
            type: 'GET',
            data: { statusFilter: statusFilter },
            success: function (data) {
                $(".my-4").html($(data).find(".my-4").html());
            },
            error: function (xhr, status, error) {
                console.error("Error fetching filtered projects:", error);
            }
        });
    }

  