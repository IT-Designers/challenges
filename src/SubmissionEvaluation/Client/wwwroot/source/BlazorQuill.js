/*eslint max-classes-per-file: "off"*/
import QuillBetterTable from "quill-better-table";
import Quill from "quill";

window.QuillFunctions = {
    createQuill: function (quillElement) {
        Quill.register({
            "modules/better-table": QuillBetterTable
        });
        const options = {
            debug: "info",
            modules: {
                toolbar: "#toolbar",
                table: false,
                history: true,
                "better-table": {
                    operationMenu: {
                        items: {
                            unmergeCells: {
                                text: "Another unmerge cells name"
                            }
                        }
                    }
                },
                keyboard: {
                    bindings: QuillBetterTable.keyboardBindings
                }
            },
            bounds: document.body,
            placeholder: "Compose an epic...",
            readOnly: false,
            theme: "snow"
        };
        //set quill at the object we can call
        //methods on later
        const quill = new Quill(quillElement, options);
        //TODO: Find another solution for that, if possible
        window.quill = quill;
        window.quill.bounds = document.body;
        //Fixing ul
        //If you feel devoted to do so: https://github.com/quilljs/quill/issues/2614
        //Table option
        const tableButton = document.querySelector(".ql-table");
        tableButton.addEventListener("click", function () {
            const range = quill.getSelection();
            if (range) {
                const tableModule = quill.getModule("better-table");
                tableModule.insertTable(3, 3);
            }
        });
        //Code block option
        const Block = Quill.import("blots/block");
        class CodeBlock extends Block { }
        CodeBlock.blotName = "codeblock";
        CodeBlock.tagName = "code";
        Quill.register(CodeBlock);

        const codeBlockBtn = document.querySelector(".ql-code-block");
        codeBlockBtn.addEventListener("click", function () {
            const range = quill.getSelection(true);
            if (range) {
                quill.insertText(range.index, "\n");
                quill.insertText(range.index + range.length + 1, "\n");
                quill.format("codeblock", true);
            }
        });
        //Horizontal rule option
        const BlockEmbed = Quill.import("blots/block/embed");
        class DividerBlot extends BlockEmbed { }
        DividerBlot.blotName = "divider";
        DividerBlot.tagName = "hr";
        Quill.register(DividerBlot);

        const horizontalRuleBtn = document.querySelector(".ql-horizontal-rule");
        horizontalRuleBtn.addEventListener("click", function () {
            const range = quill.getSelection(true);
            if (range) {
                quill.insertText(range.index, "\n", Quill.sources.USER);
                quill.insertEmbed(range.index + 1, "divider", true, Quill.sources.USER);
            }
        });
        //Terminal
        const terminalBtn = document.querySelector(".ql-console");
        terminalBtn.addEventListener("click", function () {
            replaceSelection(quill, ["{% output %}", "{% endoutput %}"]);
        });
        //Undo function
        const undoBtn = document.querySelector(".ql-undo");
        undoBtn.addEventListener("click", function () {
            quill.history.undo();
        });
        //Redo function
        const redoBtn = document.querySelector(".ql-redo");
        redoBtn.addEventListener("click", function () {
            quill.history.redo();
        });
    },
    getQuillHTML: function () {
        return window.quill.root.innerHTML;
    },
    activatePreviewUpdate: function (reference) {
        window.quill.on("text-change", () => updatePreview(reference));
    },
    deactivatePreviewUpdate: function (reference) {
        window.quill.off("text-change", () => updatePreview(reference));
    }
};
function replaceSelection(cm, startEnd) {
    const start = startEnd[0];
    const end = startEnd[1];
    const text = cm.getSelection();
    if (text) {
        cm.insertText(text.index, start);
        cm.insertText(text.index + text.length + start.length, end);
        cm.setSelection(text.index, text.length + start.length + end.length);
    }
}
function updatePreview(DotnetInstance) {
    DotnetInstance.invokeMethodAsync("UpdatePreview");
}
