var proMX = proMX || {};
proMX.dc_result = proMX.dc_result || {};
proMX.dc_result.Form = proMX.dc_result.Form || {};

proMX.dc_result.Form = function (form) {
    "use strict";

    form.HighlightLowGPAScore = function (executionContext) {
        try {
            const formContext = executionContext.getFormContext();
            const fieldNotificationId = "lowgpascore";

            let gpaAttr = formContext.getAttribute("dc_cgpa");
            let gpaControl = formContext.getControl("dc_cgpa");
            let gradeControl = formContext.getControl("dc_grade");

            // Defensive: Only alert if attribute/control truly not on the form
            if (!gpaAttr || !gpaControl) {
                return;
            }

            // Clear prior notifications
            gpaControl.clearNotification(fieldNotificationId);

            // Get GPA value
            let gpa = gpaAttr.getValue();

            // No GPA value: NG alert and exit
            if (gpa === null || gpa === undefined) {
                alert("Please generate GPA using Marks Obtained and Total Marks " + gpa);
                return;
            }

            if (parseFloat(gpa) < 6.0) {
                gpaControl.setNotification("Your GPA Score is too low", fieldNotificationId);
                if (gradeControl) {
                        gradeControl.setDisabled(true);
                }                                
            } else {
                gpaControl.clearNotification(fieldNotificationId);
                if (gradeControl) {
                    gradeControl.setDisabled(false);
                }
            }
        }
        catch (error) {
            console.error("Error in HighlightLowGPAScore: " + error.message);
        }
    };
    return form;
}(proMX.dc_result.Form);
