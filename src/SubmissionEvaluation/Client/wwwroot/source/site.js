import "bootstrap/dist/css/bootstrap.min.css";
import "bootstrap";
import "@fortawesome/fontawesome-free/css/all.css";
import $ from "jquery";

let projectChanging = true;
/*Function for enabling the Login Button after entering username + password*/
$(document).ready(() => {
    $("#alert").fadeTo(2000, 500).fadeOut(500,
        () => {
            $("#alert").fadeOut("slow");
        });

    $("#Title").on("change keyup paste",
        () => {
            if (projectChanging && (window.location.href.indexOf("Edit") === -1)) {
                $("#Name").val($("#Title").val().replace(/\s/g, "").replace("-", ""));
            }
        });

    $("#Name").on("change keyup paste",
        () => {
            projectChanging = false;
        });

    $("#SourceType").change(() => {
        HideOrShowSourceUrl();
    });

    HideOrShowSourceUrl();

    setUpSearchBar();
});

function setUpSearchBar() {
    window.searchResults = null;
    $.ajax({
        url: "/Search/Get",
        success: (result) => {
            window.searchResults = result;
        }
    });

    $("#searchBar").keyup(() => {
        const value = $("#searchBar").val().toLowerCase();
        let results = [];
        if (!(value === null || value.trim().length < 2)) {
            results = $.grep(window.searchResults,
                (elementOfArray) => elementOfArray.text.toLowerCase().indexOf(value) >= 0)
                .sort((a, b) => a.text.indexOf(value) - b.text.indexOf(value))
                .slice(0, 15);
        }

        const list = $("#searchResults");
        list.html("");
        if (results.length === 0) list.hide();
        else list.show();

        for (let i = 0; i < results.length; i++) {
            list.append(`<li class="dropdown-item"><a href="${results[i].url}" >${results[i].text}</a></li>`);
        }
    });
}

function HideOrShowSourceUrl() {
    if ($("#SourceType option:selected").text().indexOf("own") >= 0) {
        $("#SourceUrl").hide();
        $("#lblSourceUrl").hide();
        return;
    }
    $("#SourceUrl").show();
    $("#lblSourceUrl").show();
}

//Adds an input field to a container. InputForm needs to have the placeholder "%number%"
function addInputTextArea(containerId, attributeName) {
    const container = $(`#${containerId}`);
    const inputContainer = container.find("textarea");
    const inputCount = inputContainer.length;
    let id = attributeName.replace(".", "_");
    id = `${id}_${inputCount}_`;

    const name = `${attributeName}[${inputCount}]`;

    const newInput = $(`<textarea class="form-control" id="${id}" name="${name}">`);

    container.append(newInput);
}

function removeInputTextArea(element, containerId, attributeName) {
    const parent = $(element).parent();
    parent.remove();
    const container = $(`#${containerId}`);
    const inputs = container.find("textarea");
    for (let i = 0; i < inputs.length; i++) {
        $(inputs[i]).attr("name", `${attributeName}[${i}]`);
        let id = attributeName.replace(".", "_");
        id = `${id}_${i}_`;
        $(inputs[i]).attr("id", id);
    }
}

window.removeInputTextArea = removeInputTextArea;
window.addInputTextArea = addInputTextArea;
