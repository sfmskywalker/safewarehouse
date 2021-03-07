"use strict";

export function downloadFromUrl(options) {
    debugger;
    const anchorElement = document.createElement('a');
    anchorElement.href = options.url;
    anchorElement.download = options.fileName !== null && options.fileName !== void 0 ? options.fileName : '';
    anchorElement.click();
    anchorElement.remove();
}

export function downloadFromBytes(options) {
    debugger;
    const url = "data:" + options.contentType + ";base64," + options.content;
    downloadFromUrl({ url: url, fileName: options.fileName });
}
