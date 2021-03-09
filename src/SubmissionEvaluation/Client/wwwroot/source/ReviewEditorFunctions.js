import ace from "ace-builds/src-noconflict/ace";

import "ace-builds/webpack-resolver";
import $ from "jquery";
import theme from "ace-builds/src-noconflict/theme-ambiance";
import modes from "ace-builds/src-noconflict/ext-modelist";

let editor;
let currentHighlightRangeId = -1;

window.CalculatePosition = (pageX, pageY) => {
    const editorPosition = $(document.getElementById("Editorparent")).offset();
    const contextMenuStyleTop = pageY - editorPosition.top;
    const contextMenuStyleLeft = pageX - editorPosition.left;
    return [contextMenuStyleLeft, contextMenuStyleTop];
};

window.MarkCodeSmellFromSelection = (category) => {
    const range = editor.getSelectionRange();
    const marker = addMarker(range, category, "", "", null);
    const sending = createSendingPaket(marker);
    return JSON.stringify(sending);
};

window.MarkIssue = (category, id, text) => {
    const selection = editor.getSession().getSelection();
    if (!selection.isEmpty()) {
        const marker = addMarker(selection.getRange(), category, id, text, null);
        const sending = createSendingPaket(marker);
        return JSON.stringify(sending);
    }
    return "";
};

window.loadEditorForReview = (filename, content, DotnetInstance) => {
    editor = ace.edit("editor");
    ace.require("ace/ext/language_tools");
    editor.setValue(content, 1);
    editor.$blockScrolling = Infinity;
    editor.setReadOnly(true);
    editor.filename = filename;
    editor.setTheme(theme);
    const fittingMode = modes.getModeForPath(filename).mode;
    editor.getSession().setMode(fittingMode);

    //Set event listeners
    editor.getSession().selection.on("changeSelection",
        () => {
            DotnetInstance.invokeMethodAsync("HideMenu");
        });
    editor.container.addEventListener("contextmenu",
        (event) => {
            DotnetInstance.invokeMethodAsync("ShowMenu", [event.pageX, event.pageY]);
            event.preventDefault();
        },
        false);
    editor.on("mouseup",
        () => {
            DotnetInstance.invokeMethodAsync("AddCommentToIssue");
        });
};

window.removeVisualMarker = () => {
    editor.getSession().removeMarker(currentHighlightRangeId);
    currentHighlightRangeId = null;
};

window.addVisualMarker = (offset, length) => {
    const doc = editor.getSession().getDocument();
    const start = doc.indexToPosition(offset, 0);
    const end = doc.indexToPosition(offset + length, 0);
    const aceRange = new ace.Range(start.row, start.column, end.row, end.column);
    currentHighlightRangeId = editor.getSession().addMarker(aceRange, "highlight-marker");
    editor.scrollToLine(start.row, true, true, () => {});
};

window.changeContent = (filename, content) => {
    editor.setValue(content, 1);
    editor.filename = filename;
    editor.getSession().setMode(modes.getModeForPath(filename).mode);
};

window.confirmUsingSavings = () => {
    confirm("Gespeichertes Review für diese Submission gefunden. Willst du dieses fortsetzen?");
};

function addMarker(range, category, issue, text, onDelete) {
    const marker = {
        range: range,
        offset: editor.getSession().getDocument().positionToIndex(range.start),
        length: getCommentRangeLength(range),
        file: editor.filename,
        id: category, //CategoryId
        issue: issue,
        text: text,
        onDelete: onDelete
    };
    return marker;
}

function getCommentRangeLength(range) {
    const lines = editor.getSession().doc.getAllLines();
    let selectionStart = 0;
    let selectionEnd = 0;
    let selectionStartFound = false;
    const rangeStart = range.start;
    const rangeEnd = range.end;
    for (let i = 0; i < lines.length && i <= rangeEnd.row; ++i) {
        if (i === rangeStart.row) {
            selectionStart += rangeStart.column;
            selectionStartFound = true;
        } else if (!selectionStartFound) {
            selectionStart += lines[i].length + 1;
        }

        if (i === rangeEnd.row) {
            if (rangeEnd.column > lines[i].length + 1) selectionEnd += lines[i].length + 1;
            else selectionEnd += rangeEnd.column;
        } else {
            selectionEnd += lines[i].length + 1;
        }
    }
    return selectionEnd - selectionStart;
}

function createSendingPaket(marker) {
    return {
        text: marker.text,
        Id: marker.id,
        Offset: marker.offset,
        Length: marker.length,
        AssignedIssue: marker.issue
    };
}
