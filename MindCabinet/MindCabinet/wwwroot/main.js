/**
 * @param {Element} element
 * @param {string} field
 * @returns {any} Element field value.
 */
window.GetElementField = function( element, field ) {
    return element[ field ];
};

/**
 * @param {Element} element
 * @param {string} field
 * @param {any} value
 */
window.SetElementField = function( element, field, value ) {
    element[ field ] = value;
};


/* window.PopupTooltipElement = document.createElement("div");


 * @param {MouseEvent} mouseEvt
 * @param {string} text
window.PopupTooltipAtMouseElement_Display = function( mouseEvt, text ) {
    const newDiv = document.createElement("div");
    newDiv

    window.PopupTooltipElement.
    mouseEvt.pageX
};

 * @param {MouseEvent} mouseEvt
window.PopupTooltipAtMouseElement_Clear = function( mouseEvt ) {

    mouseEvt.pageX
};
*/

