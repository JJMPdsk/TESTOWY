// funkcja, która pobiera token i przesyła go dalej do requesta, którego chcemy autoryzować
// przyjmije opcjonalnie parametry prezkazywane dalej do callbacka
function authorizeRequest(request, params) {
    var baseUrl = "https://localhost:44378/Account/GetCurrentToken";
    $.ajax({
        url: baseUrl,
        method: "GET",
        success: function (token) {
            // jeśli brak parametrów, przekaż tylko token
            if (typeof params === "undefined")
                request(token);
            else
                request(token, params);
        },
        error: function () {
            console.log("Błąd podczas autoryzacji żądania")
        }
    });
}