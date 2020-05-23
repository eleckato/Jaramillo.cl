/** Load a function after jquery is loaded
 * @param {function} fn Function to execute after Jquery is loaded
*/
function WaitForJquery(fn) {
    var waitForLoad = function () {
        if (typeof jQuery != "undefined") {
            fn();
        } else {
            window.setTimeout(waitForLoad, 1);
        }
    };

    window.setTimeout(waitForLoad, 1);
}