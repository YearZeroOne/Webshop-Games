function passwordVisibleChange(inputField) {
    const passwordfield = document.getElementById(inputField);

    if (passwordfield.type === "password") {
        passwordfield.type = "text";
    }
    else {
        passwordfield.type = "password"
    }

}

document.addEventListener('DOMContentLoaded', passwordVisibleChange);
