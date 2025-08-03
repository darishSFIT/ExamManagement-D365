var proMX = proMX || {};
proMX.dc_student = proMX.dc_student || {};
proMX.dc_student.Form = proMX.dc_student.Form || {};

proMX.dc_student.Form = function (form) {
    "use strict";

    form.EnrollmentToggleCheck = async function (executionContext) {
        try {
            var formContext = executionContext.getFormContext();

            // Controls and attributes
            var courseNameControl = formContext.getControl("dc_course");
            var courseDescriptionAttribute = formContext.getAttribute("dc_coursedescription");
            var courseDescriptionControl = formContext.getControl("dc_coursedescription");
            var enrollmentStatus = formContext.getAttribute("dc_enrollmentstatus").getValue();

            if (enrollmentStatus === 121750002) {
                // Show the course and description fields
                if (courseNameControl) courseNameControl.setVisible(true);
                if (courseDescriptionControl) courseDescriptionControl.setVisible(true);

                var courseLookup = formContext.getAttribute("dc_course").getValue();

                if (courseLookup && courseLookup.length > 0) {
                    var courseId = courseLookup[0].id.replace("{", "").replace("}", "");

                    // Retrieve course description from course record using Web API
                    var courseRecord = await Xrm.WebApi.retrieveRecord("dc_course", courseId, "?$select=dc_description");

                    if (courseRecord && courseRecord.dc_description) {
                        courseDescriptionAttribute.setValue(courseRecord.dc_description);
                        formContext.setFormNotification(`description: ${courseRecord}, ${courseRecord.dc_description}`, "info");
                    } else {
                        courseDescriptionAttribute.setValue(null);
                    }
                } else {
                    // No course selected, clear description field
                    courseDescriptionAttribute.setValue(null);
                }
            } else {
                // Hide the fields and clear values when enrollment status is not the required value
                if (courseNameControl) courseNameControl.setVisible(false);
                if (courseDescriptionControl) courseDescriptionControl.setVisible(false);
                formContext.getAttribute("dc_course").setValue(null);
                courseDescriptionAttribute.setValue(null);
            }
        } catch (error) {
            console.error("Error in EnrollmentToggleCheck: " + error.message);
        }
    };
    return form;
}(proMX.dc_student.Form);
