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

/**
 * @param {Element} tooltipElem
 * @param {number} clientX
 * @param {number} clientY
 * @returns {{x: number, y: number}}
 */
window.GetTooltipPosition = ( tooltipElem, clientX, clientY ) => {
    if( !tooltipElem ) {
        return { x: clientX + 12, y: clientY + 12 };
    }

    const rect = tooltipElem.getBoundingClientRect();
    const padding = 10; // Minimum distance from screen edges

    // Default offset placement
    let x = clientX + 12;
    let y = clientY + 12;

    // Right edge collision check
    if( x + rect.width > window.innerWidth - padding ) {
        x = clientX - rect.width - 12; // Flip to the left of the cursor
    }

    // Bottom edge collision check
    if( y + rect.height > window.innerHeight - padding ) {
        y = clientY - rect.height - 12; // Flip above the cursor
    }

    // Left edge safety check
    if( x < padding ) {
        x = padding;
    }

    // Top edge safety check
    if( y < padding ) {
        y = padding;
    }

    return { x: x, y: y };
};

