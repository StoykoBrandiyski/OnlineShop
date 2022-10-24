$('#form-image').change(readURL);

function readURL() {

    const types = ['image/jpeg', 'image/png',];

    let isValid = true;

    for (let current of Object.values(this.files)) {
        if (!types.includes(current.type)) {
            isValid = false;
            break;
        }
    }

    if (!isValid) {
        $('#error-image').show().delay(4000).fadeOut();
        return;
    }
    const files = Object.values(this.files);

    if (this.files) {
        var filesAmount = this.files.length;

        for (i = 0; i < filesAmount; i++) {
            var reader = new FileReader();

            reader.onload = function (event) {
                $($.parseHTML('<img>')).attr('src', event.target.result).width(100).height(150).appendTo('.container-images');
            }

            reader.readAsDataURL(this.files[i]);
        }
    }

}