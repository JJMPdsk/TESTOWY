function getFormattedDate(date) {
    var dateObject = new Date(date);
    console.log(dateObject);
    var year = dateObject.getFullYear();

    var month = (1 + dateObject.getMonth()).toString();
    month = month.length > 1 ? month : '0' + month;

    var day = dateObject.getDate().toString();
    day = day.length > 1 ? day : '0' + day;

    return day + '/' + month + '/' + year ;
}
