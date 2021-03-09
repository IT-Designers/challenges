import ace from "ace-builds/src-noconflict/ace";

/*import "ace-builds/src-noconflict/mode-forth";
import "ace-builds/src-noconflict/mode-c_cpp";
import "ace-builds/src-noconflict/mode-kotlin";
import "ace-builds/src-noconflict/mode-java";
import "ace-builds/src-noconflict/mode-csharp";
import "ace-builds/src-noconflict/mode-fsharp";
import "ace-builds/src-noconflict/mode-haskell";
import "ace-builds/src-noconflict/mode-scala";
import "ace-builds/src-noconflict/mode-javascript";
import "ace-builds/src-noconflict/mode-perl";
import "ace-builds/src-noconflict/mode-python";
import "ace-builds/src-noconflict/mode-io";
import "ace-builds/src-noconflict/mode-golang";
import "ace-builds/src-noconflict/mode-rust";
import "ace-builds/src-noconflict/mode-typescript";
import "ace-builds/src-noconflict/mode-julia";
import "ace-builds/src-noconflict/mode-xml";
import "ace-builds/src-noconflict/mode-text";
import "ace-builds/src-noconflict/mode-toml";*/
import "ace-builds/webpack-resolver";

import theme from "ace-builds/src-noconflict/theme-ambiance";
import modes from "ace-builds/src-noconflict/ext-modelist";

window.reloadEditor = (name, content) => {
    ace.require("ace/ext/language_tools");
    const editor = ace.edit("editor");
    editor.$blockScrolling = Infinity;
    editor.setReadOnly(true);
    editor.setTheme(theme); //a theme is not required
    editor.setValue(content, -1);
    refreshEditorMode(editor, name);
};

function refreshEditorMode(editor, name) {
    //automatically find right mode for file $("#FileName")
    const fittingMode = modes.getModeForPath(name).mode;
    editor.getSession().setMode(fittingMode);
}
