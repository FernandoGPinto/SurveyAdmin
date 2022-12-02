function AddDataTable(table) {
    $(document).ready(function () {
        $(table).DataTable({
            scrollY: "350px",
            scrollX: true,
            scrollCollapse: true,
            pageLength: 10
        });
    });
}

function SaveAsFile(filename, type, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename;
    link.href = "data:" + type + ";base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}
