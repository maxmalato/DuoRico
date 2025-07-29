// Sobrescreve o método de validação de "número" do jQuery Validate
// para aceitar o formato de número brasileiro (com vírgula decimal).
$.validator.methods.number = function (value, element) {
    return this.optional(element) || /^-?(?:\d+|\d{1,3}(?:\.\d{3})*)(?:,\d+)?$/.test(value);
};