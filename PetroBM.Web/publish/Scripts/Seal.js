$(document).ready(function () {

    $("#listCommandDetail").hide();
    GetCellValues();
});

// Declare a variable to check when the action is Insert or Update
var isUpdateable = false;

function getCommanDetailByCommandID(commandID) {
    var cardSerialCommandDetail = $("#cardserial").val();
    $.ajax({
        url: '/Seal/GetCommanDetailByCommandID/',
        type: 'GET',
        data: {
            "commandID": commandID
        },
        dataType: 'json',
        success: function (data) {
            var tr = '';
            var k = 0;

            $.each(data, function (i, item) {
                var timeorder = item.TimeOrder;
                var nowtimeorder = new Date(parseInt(timeorder.substr(6)));
                var datetimeorder = new Date(nowtimeorder).toISOString();
                if (item.V_Actual == null) {
                    item.V_Actual = 0;
                }
                //          $.ajax({
                //               url: '/Seal/GetAllProduct/',
                //        type: 'GET',
                //        dataType: 'json',
                //        success: function (data2) {

                //	    $.each(data2, function (item2) {
                //                  if (item.ProduceCode == item2.ProduceCode) {
                //                      item.ProductName = item2.ProductName;
                //                  }
                //	    });
                //},
                //error: function (xhr, status, error) {
                //}
                //       });

                tr += "<tr>";
                tr += "<td style='display:none'>";
                tr += "<input  value=" + item.CompartmentOrder + " type='text'  name='[" + k + "].CompartmentOrder'></input>";
                tr += "<input  value=" + item.CardSerial + " type='text'  name='[" + k + "].CardSerial'></input>";
                tr += "<input  value=" + item.CardData + " type='text'  name='[" + k + "].CardData'></input>";
                tr += "<input  value=" + item.CommandID + " type='text'  name='[" + k + "].CommandID'></input>";
                tr += "<input  value=" + item.WorkOrder + " type='text'  name='[" + k + "].WorkOrder'></input>";
                tr += "<input  value=" + datetimeorder + " type='text'  name='[" + k + "].TimeOrder'></input>";
                tr += "<input  value=" + item.Flag + " type='text'  name='[" + k + "].Flag'></input>";
                tr += "</td>";

                tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;'readonly value=" + item.CompartmentOrder + " type='text' name='[" + k + "].CommandCode'></input></td>";
                tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;'readonly value=" + item.V_Preset + " type='text' name='[" + k + "].Vtt'></input></td>";

                //for (var j = 0; j < lstProduct.length; j++) {
                //    if (item.ProduceCode == lstProduct[j].type) {

                //    }

                //}
                tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;'readonly value='" + item.ProductName + "' type='text' name='[" + k + "].ProductName'></input></td>";
                tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;'readonly value=" + item.V_Actual + " type='text'  name='[" + k + "].VolumeCode'></input></td>";
                tr += "<td><input style='width:100%; text-align:center' value='' type='text' class='inputvalue' name='[" + k + "].Seal1'></input></td>";
                tr += "<td><input style='width:100%; text-align:center' value='' type='text'  class='inputvalue'  name='[" + k + "].Seal2'></input></td>";
                tr += "<td><input style='width:100%; text-align:center' value='0' type='number'  class='inputvalue' name='[" + k + "].Ratio' ></input></td>";
                tr += "</tr>";
                k = k + 1;
            });
            $("#listCommandDetail tbody").html(tr);
            $("#listCommandDetail").show();
        },
        error: function (xhr, status, error) {

            //alert("Error: " + err.responseText);
            //alert("getCommanDetailByCommandID error!");
            //$("#listCommandDetail tbody").html("");
            //$("#listCommandDetail").show();
        }
    });

}

//20181107 edit getCommandByCardSerial(cardSerial) -> getCommandByCardSerial()

function getCommandByCardSerial() {
    var cardSerial = $("#cardserial").val();
    //alert(cardSerial);
    $.ajax({
        url: '/Seal/GetCardDataByCardSerial/',
        type: 'GET',
        data: {
            "cardSerial": cardSerial
        },
        dataType: 'json',
        success: function (data) {
            if (data === null) {
                //alert("data is null");
            } else if (data === "") {
                //alert("data is blank");
                document.getElementById('warningseal').textContent = "Card Serial không tồn tại";
                document.getElementById('buttonsubmit').style.visibility = "hidden";
                document.getElementById('cancle').style.visibility = "hidden";
                $("#listCommandDetail").hide();
                document.getElementById('SealLast').value = "";
                GetDataError();
                return;
            }
            document.getElementById('warningseal').textContent = "";
            document.getElementById('buttonsubmit').style.visibility = "visible";
            document.getElementById('cancle').style.visibility = "visible";


            console.log(data);
            commandID = data.CommandID;
            getCommanDetailByCommandID(commandID);

            //var date = data.TimeOrder;
            //var nowDate = new Date(parseInt(date.substr(6)));

            //document.getElementById('warehousecodecommand').value = data.WareHouseCode;
            //  document.getElementById('timeordercommand').value = data.TimeOrder; //Old
            document.getElementById('codecommand').value = data.WorkOrder;
            document.getElementById('cardData').value = data.CardData;
            document.getElementById('vehiclenumber').value = data.VehicleNumber;
            document.getElementById('drivername').value = data.DriverName;
            document.getElementById('customercode').value = data.CustomerCode;
            var date = data.TimeOrder;
            var nowDate = new Date(parseInt(date.substr(6)));

            document.getElementById('timeordercommand').value = GetDateVN(nowDate);

        },
        error: function (err) {
            //alert("getCommandByCardSerial Error: " + err.responseText);
            //$("#listCommandDetail tbody").html("");
            //$("#listCommandDetail").show();

            GetDataError();
        }
    });
}

function ValidateData() {
    var seallast = document.getElementById('SealLast').value;
    if (seallast == null || seallast == "") {

        return false;
    }
    return true;

}

function getCommandByCardData(cardData) {
    var cardData = $("#cardData").val();
    $.ajax({
        url: '/Seal/GetCardSerialByCardData/',
        type: 'GET',
        data: {
            "cardData": cardData
        },
        dataType: 'json',
        success: function (data) {
            var a = data;

            for (var i = 0; i < a.length; i++) {
                getCommands();
                //  document.getElementById('warehousecodecommand').value = a[i].WareHouseCode;
                document.getElementById('timeordercommand').value = a[i].TimeOrder;
                document.getElementById('codecommand').value = a[i].CommandCode;
                document.getElementById('cardserial').value = a[i].CardSerial;
                document.getElementById('vehiclenumber').value = a[i].VehicleNumber;
                document.getElementById('drivername').value = a[i].DriverName;
                document.getElementById('customercode').value = a[i].CustomerCode;

                var date = a[i].TimeOrder;
                var nowDate = new Date(parseInt(date.substr(6)));
                var datecommand = new Date(nowDate).toLocaleString();

                document.getElementById('timeordercommand').value = datecommand;

            }
        },
        error: function (err) {
            //alert("getCommandByCardData Error: " + err.responseText);
        }
    });
}

function getCommandByCode() {
    var commandeCode = $("#codecommand").val();
    $.ajax({
        url: '/Seal/GetCardDataByWorkOrder/',
        type: 'GET',
        data: {
            "commandeCode": commandeCode
        },
        dataType: 'json',
        success: function (data) {
            if (data == null) {
                //alert("data is null");
            } else if (data == "") {
                //alert("data is blank");
                document.getElementById('warningseal').textContent = "Lệnh không tồn tại";
                document.getElementById('buttonsubmit').style.visibility = "hidden";
                document.getElementById('cancle').style.visibility = "hidden";
                $("#listCommandDetail").hide();
                document.getElementById('SealLast').value = "";
                GetDataError();
                return;
            }
            document.getElementById('warningseal').textContent = "";
            document.getElementById('buttonsubmit').style.visibility = "visible";
            document.getElementById('cancle').style.visibility = "visible";

            console.log(data);
            commandID = data.CommandID;
            getCommanDetailByCommandID(commandID);

            document.getElementById('codecommand').value = data.WorkOrder;
            document.getElementById('cardData').value = data.CardData;
            document.getElementById('cardserial').value = data.CardSerial;
            document.getElementById('vehiclenumber').value = data.VehicleNumber;
            document.getElementById('drivername').value = data.DriverName;
            document.getElementById('customercode').value = data.CustomerCode;
            var date = data.TimeOrder;
            var nowDate = new Date(parseInt(date.substr(6)));

            document.getElementById('timeordercommand').value = GetDateVN(nowDate);
        },
        error: function (err) {
            GetDataError();
        }
    });
}

function Customer() {
    $.ajax({
        url: '/Seal/GetAllCustomer/',
        type: 'GET',
        dataType: 'json',
        success: function (data) {
            var tr = '';
            $.each(data, function (i, item) {
                var options = {
                    data: obj,

                    getValue: "CustomerCode",

                    template: {
                        type: "description",
                        fields: {
                            description: "ID"
                        }
                    },

                    list: {
                        match: {
                            enabled: true
                        }
                    },

                    theme: "plate-dark"
                };

                $("#customercodes").easyAutocomplete(options);
            });
        },
        error: function (err) {
            //alert("Customer Error: " + err.responseText);
        }
    });
}

function Textchange() {
    var a = document.getElementById("SealLast").value;

    if (isNaN(document.getElementById("SealLast").value)) {
        return;
    }
    var indexNo = Number(document.getElementById("SealLast").value);
    var i = 0
    $('#listCommandDetail').find('.inputvalue').each(function () {
        if (a == "") {
            if (i % 3 != 2) {
                $(this).val('');
            }
        }
        else {
            if (i % 3 != 2) {
                $(this).val(indexNo + 1);
                indexNo = indexNo + 1;
            }
        }
        i = i + 1;
    });


    return;
};


var myvar = "";
$("#btnPrint").click(function (e) {
    colSum();
})

var dataseal = new Array();
var newArr = new Array();
var sealarray = new Array();
jsondata = [];
$("#btnSave").click(function (e) {


    Getdatatable();
    for (var i = 0; i < dataseal.length; i++) {
        var subArray = dataseal[i],
            item = {
                CommandCode: subArray[0],
                VolumeCode: subArray[1],
                ProduceCode: subArray[3],
                Seal1: subArray[4],
                Seal2: subArray[5],
                Ratio: subArray[6],
            };
        jsondata.push(item);
        console.log(jsondata);
    }

    $.post("/Seal/RegisterSeal", { strPara: JSON.stringify(jsondata) }, function (data) {

    });
});

function Getdatatable() {
    $("table#listCommandDetail tr").each(function () {
        var arrayOfThisRow = [];
        var tableData = $(this).find('td');
        if (tableData.length > 0) {
            tableData.each(function () { arrayOfThisRow.push($(this).text()); });
            dataseal.push(arrayOfThisRow);
        }
    });
    var i = 0;
    var intRow = 0;
    var intCol = 0
    $('#listCommandDetail').find('input[type=text],select').each(function () {
        dataseal[intRow][intCol + 4] = $(this).val();
        intCol = intCol + 1;
        i = i + 1;
        if (i % 3 == 0) {
            intRow = intRow + 1;
            intCol = 0;
        }

    });
}

function colSum() {
    var sum = 0;
    var arrVolume = [];
    var intRowVolume = 0;
    var intCol = 0;
    var k = 0;
    var intcount = 0;

    $('#listCommandDetail').find('input[type=text],select').each(function () {
        if (intcount % 9 == 0) {
            arrVolume[intRowVolume] = [];
        }
        if ($(this).val().length > 0) {
            arrVolume[intRowVolume][intCol] = $(this).val();
        }
        else {
            arrVolume[intRowVolume][intCol] = '';
        }
        intcount = intcount + 1;
        intCol = intCol + 1;

        if (intcount % 9 == 0) {
            intRowVolume = intRowVolume + 1;
            intCol = 0;
        }
    })
    for (var i = 0; i < intRowVolume; i++) {
        if (arrVolume[i][2] != "") {
            dt = dt + eval(arrVolume[i][1]);
            sn = sn + 1;
        }
        document.getElementById("lvlTotalV_Actual").innerHTML = String(dt);
    }
}

function GetCellValues() {

}


function GetDataError() {
    // document.getElementById('warehousecodecommand').value = "";
    document.getElementById('timeordercommand').value = "";
    document.getElementById('cardData').value = "";
    document.getElementById('vehiclenumber').value = "";
    document.getElementById('drivername').value = "";
    document.getElementById('customercode').value = "";
    $("#listCommandDetail tbody").html("");
    $("#listCommandDetail").hide();
}

function GetDateVN(date) {

    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var yyyy = date.getFullYear();
    var mim = date.getMinutes();
    var hh = date.getHours();

    var str = '';
    if (dd < 10)
        str += '0' + dd;
    else
        str += '' + dd;

    if (mm < 10)
        str += '/0' + mm;
    else
        str += '' + mm;

    if (hh < 10) {
        str += '/' + yyyy + ' 0' + hh;
    }
    else {
        str += '/' + yyyy + ' ' + hh;
    }

    if (mim < 10)
        str += ':0' + mim;
    else
        str += ':' + mim;

    return str;
}
