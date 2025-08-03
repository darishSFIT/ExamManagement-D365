var proMX = proMX || {};
proMX.dc_exam = proMX.dc_exam || {};
proMX.dc_exam.Form = proMX.dc_exam.Form || {};

proMX.dc_exam.Form = function (form) {
    "use strict";

    form.BlockPastExamDate = function (executionContext) {
        try {

            var formContext = executionContext.getFormContext();
            var examDateControl = formContext.getControl("dc_examdate");
            var examDateAttribute = formContext.getAttribute("dc_examdate");

            const fieldNotificationId = "examDatePast";
            const formNotificationId = "examDateMissing";

            // checks to ensure the control and field exist
            if (!examDateControl || !examDateAttribute) {
                alert("Exam date field not found on form");
                return;
            }

            // Clear prior notifications
            examDateControl.clearNotification(fieldNotificationId);
            formContext.ui.clearFormNotification(formNotificationId);

            // Get the exam date value for comparison
            var examDate = examDateAttribute.getValue();

            // when a date is selected, OnChange event is triggered
            if (examDate !== null && examDate !== undefined) {
                var today = new Date();
                today.setHours(0, 0, 0, 0);

                var selectedDate = new Date(examDate);
                selectedDate.setHours(0, 0, 0, 0);

                // Compare selected date with today's date
                if (selectedDate < today) {
                    const yyyy = selectedDate.getFullYear();
                    let mm = selectedDate.getMonth() + 1;
                    let dd = selectedDate.getDate();
                    if (dd < 10) dd = '0' + dd;
                    if (mm < 10) mm = '0' + mm;
                    const formattedSelectedDate = dd + '/' + mm + '/' + yyyy;

                    // show error notification on the exam date field
                    examDateControl.setNotification(
                        `Exam date cannot be in the past (${formattedSelectedDate}). Please select a current or future date.`,
                        fieldNotificationId 
                    );

                    // Prevent onChange loop
                    examDateAttribute.setValue(null, false);

                } else { // if the date input is valid, set focus on the exam duration field
                    var durationCtrl = formContext.getControl("dc_durationhours");
                    if (durationCtrl) durationCtrl.setFocus();                    
                }
            } else { // if no date is selected, show a form notification
                formContext.ui.setFormNotification("Please select an exam date.", "ERROR", formNotificationId);
            }
        }
        catch (error) {
            console.error("Error in BlockPastExamDate: " + error.message);
        }
    };
    return form;
}(proMX.dc_exam.Form);
