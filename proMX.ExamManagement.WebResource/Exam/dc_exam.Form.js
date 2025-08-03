var proMX = proMX || {};
proMX.dc_exam = proMX.dc_exam || {};
proMX.dc_exam.Form = proMX.dc_exam.Form || {};

proMX.dc_exam.Form = function (form) {
    "use strict";

    form.BlockPastExamDate = function (executionContext) {
        try {
            var formContext = executionContext.getFormContext();
            //var examDateControl = formContext.getControl("dc_examdate");
            var examDateAttribute = formContext.getAttribute("dc_examdate");

            // Add null checks for defensive programming
            if (!examDateControl || !examDateAttribute) {
                console.warn("Exam date field not found on form");
                return;
            }

            var examDate = examDateAttribute.getValue();

            // Always clear notifications first
            formContext.ui.clearFormNotification("err1");
            formContext.getControl("dc_examdatey")?.clearNotification();

            if (examDate !== null && examDate !== undefined) {
                var today = new Date();
                today.setHours(0, 0, 0, 0);

                var selectedDate = new Date(examDate);
                selectedDate.setHours(0, 0, 0, 0);

                if (selectedDate < today) {
                    const yyyy = selectedDate.getFullYear();
                    let mm = selectedDate.getMonth() + 1;
                    let dd = selectedDate.getDate();
                    if (dd < 10) dd = '0' + dd;
                    if (mm < 10) mm = '0' + mm;
                    const formattedSelectedDate = dd + '/' + mm + '/' + yyyy;

                    formContext.getControl("dc_examdate")?.setNotification(
                        `Exam date cannot be in the past (${formattedSelectedDate}). Please select a current or future date.`,
                        "ERROR",
                        "examDateError"
                    );

                    // Clear the invalid date but don't fire onChange to prevent loop
                    examDateAttribute.setValue(null, false);
                }
                // Remove the else if condition - notification is already cleared at the top
            }
            // Remove the final else condition - it's unnecessary
        }
        catch (error) {
            console.error("Error in BlockPastExamDate: " + error.message);
        }
    };

    return form;
}(proMX.dc_exam.Form);
