

function getCommandByCardSerial() {
	var cardSerial = $("#cardSerial").val();
	$.ajax({
		url: '/Bills/GetDataByCardSerial/',
		type: 'GET',
		data: {
			"cardSerial": cardSerial
		},
		dataType: 'json',
		success: function (data) {
			document.getElementById('warningbill').textContent = "";
			document.getElementById('buttonsubmit').style.visibility = "visible";
			document.getElementById('billredirectaction').style.visibility = "visible";
			getCommandsByCardSerial(cardSerial);

			var date = data.TimeOrder;
			var nowDate = new Date(parseInt(date.substr(6)));
			//var datecommand = new Date(nowDate).toLocaleString();

			//alert(GetDateVN(nowDate));

            document.getElementById('cardData').value = data.CardData;
			document.getElementById('timebillcheckout').value = GetDateVN(nowDate);
			document.getElementById('vehiclenumber').value = data.VehicleNumber;
			document.getElementById('drivername').value = data.DriverName;
			document.getElementById('commandcode').value = data.WorkOrder;
		},
		error: function (err) {
			document.getElementById('warningbill').textContent = "Card Serial không tồn tại";
			$("#listCommandDetail").hide();
			GetDataError();
			document.getElementById('buttonsubmit').style.visibility = "hidden";
			document.getElementById('billredirectaction').style.visibility = "hidden";
		}
	});
}

function getCommandByCommandCode() {
	var workOrder = $("#commandcode").val();
	$.ajax({
		url: '/Bills/GetDataByWorkOrder/',
		type: 'GET',
		data: {
			"workOrder": workOrder
		},
		dataType: 'json',
		success: function (data) {
			document.getElementById('warningbill').textContent = "";
			document.getElementById('buttonsubmit').style.visibility = "visible";
			document.getElementById('billredirectaction').style.visibility = "visible";
			getCommandsByCommandCode(workOrder);

			var date = data.TimeOrder;
			var nowDate = new Date(parseInt(date.substr(6)));
			//var datecommand = new Date(nowDate).toLocaleString();

			//alert(GetDateVN(nowDate));

            document.getElementById('cardData').value = data.CardData;
            document.getElementById('cardSerial').value = data.CardSerial;
			document.getElementById('timebillcheckout').value = GetDateVN(nowDate);
			document.getElementById('vehiclenumber').value = data.VehicleNumber;
			document.getElementById('drivername').value = data.DriverName;
			document.getElementById('commandcode').value = data.WorkOrder;
		},
		error: function (err) {
			document.getElementById('warningbill').textContent = "Lệnh không tồn tại";
			$("#listCommandDetail").hide();
			GetDataError();
			document.getElementById('buttonsubmit').style.visibility = "hidden";
			document.getElementById('billredirectaction').style.visibility = "hidden";
		}
	});
}

function GetDataError() {
	document.getElementById('cardData').value = "";
	document.getElementById('timebillcheckout').value = "";
	document.getElementById('vehiclenumber').value = "";
	document.getElementById('drivername').value = "";
	document.getElementById('commandcode').value = "";

}

function ValidateData() {
	var carddata = document.getElementById('cardData').value;
	if (carddata == null || carddata == "") {
		return false;
	}
	return true;
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
		str += '/' + mm;

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

function getCommandsByCardSerial(cardSerial) {
	$.ajax({
		url: '/Bills/GetCommanDetailByCardSerial/',
		type: 'GET',
		data: {
			"cardSerial": cardSerial
		},
		dataType: 'json',
		success: function (data) {
			if (data == null || data == "") {
				// $('#buttonsubmit').prop('disabled', true);
				return;
			};
			var tr = '';
			var k = 0;
			var armno = data[0].ArmNo;

            $.each(data, function (i, item) {
                if (item.Flag === 4 || item.Flag === 0 || item.Flag === 1 || item.Flag === 2 || item.Flag === 6) {
                    var AvgTemperature = 0;
                    var AvgDensity = 0;
                    var timeorder = item.TimeOrder;
                    var nowtimeorder = new Date(parseInt(timeorder.substr(6)));
                    var datetimeorder = new Date(nowtimeorder).toISOString();

                    if (item.TimeStart !== null) {
                        var timestart = item.TimeStart;
                        var nowtimestart = new Date(parseInt(timestart.substr(6)));
                        var datetimestart = new Date(nowtimestart).toISOString();
                    }

                    if (item.TimeStop !== null) {
                        var timestop = item.TimeStop;
                        var nowtimestop = new Date(parseInt(timestop.substr(6)));
                        var datetimestop = new Date(nowtimestop).toISOString();
                    }

                    var insertdate = item.InsertDate;
                    var nowinsertdate = new Date(parseInt(insertdate.substr(6)));
                    var dateinsertdate = new Date(nowinsertdate).toISOString();

                    var updatedate = item.InsertDate;
                    var nowupdatedate = new Date(parseInt(updatedate.substr(6)));
                    var dateupdatedate = new Date(nowinsertdate).toISOString();

                    if (item.AvgTemperature !== null) {
                        AvgTemperature = item.AvgTemperature;
                    }
                    if (item.AvgDensity !== null) {
                        AvgDensity = item.AvgDensity;
                    }
                    if (item.TankID !== null) {
                        TankID = item.TankID;
                    }

                    tr += "<tr>"
                    tr += "<td style='display:none'><input value=" + item.ID + " type='text' name='[" + k + "].ID'></input> ";
                    tr += "<input value=" + item.CommandID + " type='text' name='[" + k + "].CommandID'></input>";
                    tr += "<input value=" + item.CommandCode + " type='text' name='[" + k + "].CommandCode'></input>";
                    tr += "<input value=" + item.ProductCode + " type='text' name='[" + k + "].ProductCode'></input>";
                    tr += "<input value=" + datetimeorder + " type='text' name='[" + k + "].TimeOrder'></input>";
                    tr += "<input value=" + datetimestart + " type='text' name='[" + k + "].TimeStart'></input>";
                    tr += "<input value=" + datetimestop + " type='text' name='[" + k + "].TimeStop'></input>";
                    tr += "<input value=" + item.WorkOrder + " type='text' name='[" + k + "].WorkOrder'></input>";
                    tr += "<input value=" + item.WareHouseCode + " type='text' name='[" + k + "].WareHouseCode'></input>";
                    tr += "<input value=" + item.ArmNo + " type='text' name='[" + k + "].ArmNo'></input>";
                    tr += "<input value=" + item.CardData + " type='text' name='[" + k + "].CardData'></input>";
                    tr += "<input value=" + item.CardSerial + " type='text' name='[" + k + "].CardSerial'></input>";
                    tr += "<input value=" + item.V_Deviation + " type='text' name='[" + k + "].V_Deviation'></input>";
                    tr += "<input value=" + item.CurrentTemperature + " type='text' name='[" + k + "].CurrentTemperature'></input>";
                    tr += "<input value=" + item.Flag + " type='text' name='[" + k + "].Flag'></input>";
                    tr += "<input value=" + item.TotalStart + " type='text' name='[" + k + "].TotalStart'></input>";
                    tr += "<input value=" + item.TotalEnd + " type='text' name='[" + k + "].TotalEnd'></input>";
                    tr += "<input value=" + item.Vehicle + " type='text' name='[" + k + "].Vehicle'></input>";
                    tr += "<input value=" + item.MixingRatio + " type='text' name='[" + k + "].MixingRatio'></input>";
                    tr += "<input value=" + item.GasDensity + " type='text' name='[" + k + "].GasDensity'></input>";
                    tr += "<input value=" + item.AlcoholicDensity + " type='text' name='[" + k + "].AlcoholicDensity'></input>";
                    tr += "<input value=" + item.V_Actual_15 + " type='text' name='[" + k + "].V_Actual_15'></input>";;
                    tr += "<input value=" + item.TotalStart_15 + " type='text' name='[" + k + "].TotalStart_15'></input>";
                    tr += "<input value=" + item.TotalEnd_15 + " type='text' name='[" + k + "].TotalEnd_15'></input>";
                    tr += "<input value=" + item.V_Actual_Base + " type='text' name='[" + k + "].V_Actual_Base'></input>";
                    tr += "<input value=" + item.V_Actual_Base_15 + " type='text' name='[" + k + "].V_Actual_Base_15'></input>";
                    tr += "<input value=" + item.V_Actual_E_15 + " type='text' name='[" + k + "].V_Actual_E_15'></input>";
                    tr += "<input value=" + item.TotalStart_Base + " type='text' name='[" + k + "].TotalStart_Base'></input>";
                    tr += "<input value=" + item.TotalStart_E + " type='text' name='[" + k + "].TotalStart_E'></input>";
                    tr += "<input value=" + item.TotalStart_Base_15 + " type='text' name='[" + k + "].TotalStart_Base_15'></input>";
                    tr += "<input value=" + item.TotalStart_E_15 + " type='text' name='[" + k + "].TotalStart_E_15'></input>";
                    tr += "<input value=" + item.TotalEnd_Base + " type='text' name='[" + k + "].TotalEnd_Base'></input>";
                    tr += "<input value=" + item.TotalEnd_E + " type='text' name='[" + k + "].TotalEnd_E'></input>";
                    tr += "<input value=" + item.TotalEnd_Base_15 + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>";
                    tr += "<input value=" + item.TotalEnd_E_15 + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>";
                    tr += "<input value=" + item.CTL_Base + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>";
                    tr += "<input value=" + item.CTL_E + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>";
                    tr += "<input value=" + dateinsertdate + " type='text' name='[" + k + "].InsertDate'></input>";
                    tr += "<input value=" + item.InsertUser + " type='text' name='[" + k + "].InsertUser'></input>";
                    tr += "<input value=" + dateupdatedate + " type='text' name='[" + k + "].UpdateDate'></input>";
                    tr += "<input value=" + item.UpdateUser + " type='text' name='[" + k + "].UpdateUser'></input>";
                    tr += "<input value=" + item.VersionNo + " type='text' name='[" + k + "].VersionNo'></input>";
                    tr += "<input value=" + item.DeleteFlg + " type='text' name='[" + k + "].DeleteFlg'></input>";
                    tr += "<input value=" + item.ActualRatio + " type='text' name='[" + k + "].ActualRatio'></input>";
                    tr += "</td>"
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9' value=" + item.CompartmentOrder + " type='text' name='[" + k + "].CompartmentOrder' readonly></input></td>";
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + item.V_Preset + " type='text'  name='[" + k + "].V_Preset' readonly></input></td>";
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value='" + item.ProductName + "' type='text'  name='[" + k + "].ProductName' readonly></input></td>";
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + item.V_Preset + " type='text'  name='[" + k + "].V_Actual' tabindex = '2'></input></td>";
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + AvgTemperature + " type='text'  name='[" + k + "].AvgTemperature'tabindex = '3'></input></td>";
                    tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + AvgDensity + " type='text'  name='[" + k + "].AvgDensity'tabindex = '4'></input></td>";

                    tr += "</tr>";
                    $("#listCommandDetail tbody").html(tr);
                    $("#listCommandDetail").show();
                    k = k + 1;

                    // $('#buttonsubmit').prop('disabled', false);
                }
            });
		},
		error: function (err) {
			alert("Error: " + err.responseText);
		}
	});
}


function getCommandsByCommandCode(workOrder) {
    $.ajax({
        url: '/Bills/GetCommanDetailByWorkOrder/',
        type: 'GET',
        data: {
            "workOrder": workOrder
        },
        dataType: 'json',
        success: function (data) {
            if (data == null || data == "") {
                // $('#buttonsubmit').prop('disabled', true);
                return;
            };
            var tr = '';
            var k = 0;
            var armno = data[0].ArmNo;

            $.ajax({
                url: '/Bills/GetDataByArmno/',
                type: 'GET',
                data: {
                    "armno": armno
                },
                dataType: 'json',
                success: function (databyarmno) {
                    var AvgTemperature = databyarmno.AvgTemperature;
                    var AvgDensity = databyarmno.AvgDensity;

                    $.each(data, function (i, item) {
                        if (item.Flag == 4) {
                            var timeorder = item.TimeOrder;
                            var nowtimeorder = new Date(parseInt(timeorder.substr(6)));
                            var datetimeorder = new Date(timeorder).toISOString();

                            var timestart = item.TimeStart;
                            var nowtimestart = new Date(parseInt(timestart.substr(6)));
                            var datetimestart = new Date(nowtimestart).toISOString();

                            var timestop = item.TimeStop;
                            var nowtimestop = new Date(parseInt(timestop.substr(6)));
                            var datetimestop = new Date(nowtimestop).toISOString();

                            var insertdate = item.InsertDate;
                            var nowinsertdate = new Date(parseInt(insertdate.substr(6)));
                            var dateinsertdate = new Date(nowinsertdate).toISOString();

                            var updatedate = item.InsertDate;
                            var nowupdatedate = new Date(parseInt(updatedate.substr(6)));
                            var dateupdatedate = new Date(nowinsertdate).toISOString();

                            if (AvgTemperature == null) {
                                AvgTemperature = 0;
                            }
                            if (AvgDensity == null) {
                                AvgDensity = 0;
                            }

                            tr += "<tr>"
                            tr += "<td style='display:none'><input value=" + item.ID + " type='text' name='[" + k + "].ID'></input> "
                            tr += "<input value=" + item.CommandID + " type='text' name='[" + k + "].CommandID'></input>"
                            tr += "<input value=" + item.CommandCode + " type='text' name='[" + k + "].CommandCode'></input>"
                            tr += "<input value=" + item.ProductCode + " type='text' name='[" + k + "].ProductCode'></input>"
                            tr += "<input value=" + datetimeorder + " type='text' name='[" + k + "].TimeOrder'></input>"
                            tr += "<input value=" + datetimestart + " type='text' name='[" + k + "].TimeStart'></input>"
                            tr += "<input value=" + datetimestop + " type='text' name='[" + k + "].TimeStop'></input>"
                            tr += "<input value=" + item.WorkOrder + " type='text' name='[" + k + "].WorkOrder'></input>"
                            tr += "<input value=" + item.WareHouseCode + " type='text' name='[" + k + "].WareHouseCode'></input>"
                            tr += "<input value=" + item.ArmNo + " type='text' name='[" + k + "].ArmNo'></input>"
                            tr += "<input value=" + item.CardData + " type='text' name='[" + k + "].CardData'></input>"
                            tr += "<input value=" + item.CardSerial + " type='text' name='[" + k + "].CardSerial'></input>"
                            tr += "<input value=" + item.V_Deviation + " type='text' name='[" + k + "].V_Deviation'></input>"
                            tr += "<input value=" + item.CurrentTemperature + " type='text' name='[" + k + "].CurrentTemperature'></input>"
                            tr += "<input value=" + item.Flag + " type='text' name='[" + k + "].Flag'></input>"
                            tr += "<input value=" + item.TotalStart + " type='text' name='[" + k + "].TotalStart'></input>"
                            tr += "<input value=" + item.TotalEnd + " type='text' name='[" + k + "].TotalEnd'></input>"
                            tr += "<input value=" + item.Vehicle + " type='text' name='[" + k + "].Vehicle'></input>"
                            tr += "<input value=" + item.MixingRatio + " type='text' name='[" + k + "].MixingRatio'></input>"
                            tr += "<input value=" + item.GasDensity + " type='text' name='[" + k + "].GasDensity'></input>"
                            tr += "<input value=" + item.AlcoholicDensity + " type='text' name='[" + k + "].AlcoholicDensity'></input>"
                            tr += "<input value=" + item.V_Actual_15 + " type='text' name='[" + k + "].V_Actual_15'></input>"
                            tr += "<input value=" + item.TotalStart_15 + " type='text' name='[" + k + "].TotalStart_15'></input>"
                            tr += "<input value=" + item.TotalEnd_15 + " type='text' name='[" + k + "].TotalEnd_15'></input>"
                            tr += "<input value=" + item.V_Actual_Base + " type='text' name='[" + k + "].V_Actual_Base'></input>"
                            tr += "<input value=" + item.V_Actual_Base_15 + " type='text' name='[" + k + "].V_Actual_Base_15'></input>"
                            tr += "<input value=" + item.V_Actual_E_15 + " type='text' name='[" + k + "].V_Actual_E_15'></input>"
                            tr += "<input value=" + item.TotalStart_Base + " type='text' name='[" + k + "].TotalStart_Base'></input>"
                            tr += "<input value=" + item.TotalStart_E + " type='text' name='[" + k + "].TotalStart_E'></input>"
                            tr += "<input value=" + item.TotalStart_Base_15 + " type='text' name='[" + k + "].TotalStart_Base_15'></input>"
                            tr += "<input value=" + item.TotalStart_E_15 + " type='text' name='[" + k + "].TotalStart_E_15'></input>"
                            tr += "<input value=" + item.TotalEnd_Base + " type='text' name='[" + k + "].TotalEnd_Base'></input>"
                            tr += "<input value=" + item.TotalEnd_E + " type='text' name='[" + k + "].TotalEnd_E'></input>"
                            tr += "<input value=" + item.TotalEnd_Base_15 + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>"
                            tr += "<input value=" + item.TotalEnd_E_15 + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>"
                            tr += "<input value=" + item.CTL_Base + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>"
                            tr += "<input value=" + item.CTL_E + " type='text' name='[" + k + "].TotalEnd_Base_15'></input>"
                            tr += "<input value=" + dateinsertdate + " type='text' name='[" + k + "].InsertDate'></input>"
                            tr += "<input value=" + item.InsertUser + " type='text' name='[" + k + "].InsertUser'></input>"
                            tr += "<input value=" + dateupdatedate + " type='text' name='[" + k + "].UpdateDate'></input>"
                            tr += "<input value=" + item.UpdateUser + " type='text' name='[" + k + "].UpdateUser'></input>"
                            tr += "<input value=" + item.VersionNo + " type='text' name='[" + k + "].VersionNo'></input>"
                            tr += "<input value=" + item.DeleteFlg + " type='text' name='[" + k + "].DeleteFlg'></input>"
                            tr += "<input value=" + item.ActualRatio + " type='text' name='[" + k + "].ActualRatio'></input>"
                            tr += "</td>"
                            tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9' value=" + item.CompartmentOrder + " type='text' name='[" + k + "].CompartmentOrder' readonly></input></td>"
                            tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + item.V_Preset + " type='text'  name='[" + k + "].V_Preset' readonly></input></td>"
                            tr += "<td><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + item.ProductName + " type='text'  name='[" + k + "].ProductName' readonly></input></td>"
                            tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + item.V_Preset + " type='text'  name='[" + k + "].V_Actual' tabindex = '2'></input></td>"
                            tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + AvgTemperature + " type='text'  name='[" + k + "].AvgTemperature'tabindex = '3'></input></td>"
                            tr += "<td ><input style='width:100%;text-align:center; border:#fff;background-color:#f9f9f9;' value=" + AvgDensity + " type='text'  name='[" + k + "].AvgDensity'tabindex = '4'></input></td>"

                            tr += "</tr>";
                            $("#listCommandDetail tbody").html(tr);
                            $("#listCommandDetail").show();
                            k = k + 1;
                        }

                    });
                },
                error: function (err) {
                    alert("Error: " + err.responseText);
                },
            });
        },
        error: function (err) {
            alert("Error: " + err.responseText);
        }
    });
}