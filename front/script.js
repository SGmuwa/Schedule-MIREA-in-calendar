$('document').ready(function(){

	var flagForSettings = false;
	var flagForTimezones = false;
	var flagForDiscription = false;
	var flagForObj = false;
	var valueForTimezone = 'cool';
	var windowHeight = window.innerHeight;
	var height = ((window.innerHeight / 100) * 130) + 'px';
	var zonesArr = [];
	var srcOfWeb = document.location.href;
	var obj = {

	    name: "ИКБО-04-16",
	    dateStart: "09/01/2018",
	    dateFinish: "11/30/2018",
	    timeZone: "UTC +5"
	};
	var objForSrc = {};

	$('#modalForLoader').fadeOut([10],[]);
	$('#modalForLoader').fadeIn();

	/*$.get(
  		srcOfWeb,{

    		data: ""
  		},
  	onAjaxSuccess
	);*/

	$.get(srcOfWeb, function(data){
        zonesArr = data;
  		console.log(data);
		$('#main').fadeIn();
		$('#modalForLoader').fadeOut();
    });
 
	/*function onAjaxSuccess(data){
  
  		zonesArr = data;
  		console.log(data);
		$('#main').fadeIn();
		$('#modalForLoader').fadeOut();
	}*/


  	$(function(){
    	$('#datepickerStart').datepicker({
    		changeMonth: true
   		});
  	});
  	$(function(){
    	$('#datepickerFinish').datepicker({
			changeMonth: true
    	});
  	});

	for(var i = 0; i < 601; i++){

		$('<div/>', {

			class: 'timezones',
			id: 'zone' + i,
		}).appendTo('#divForTimezones');
		document.getElementById('zone' + i).innerHTML += zonesArr[i];
	}

	$('#timezone').on('click', function(){
		
		if(flagForTimezones == false){

			$('#divForTimezones').fadeIn();
			$('#supportDivForBottomBorder').fadeIn();
			$('body').css('height', height);
			flagForTimezones = true;
		}else{

			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});

	$('.timezones').on('click', function(){

		var id = $(this).attr('id');
		id = id.replace('zone', '');
		$('#timezone').attr('value', zonesArr[id]);
		if(flagForTimezones == true){

			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});

	$('.cogSettings').on('click', function(){

		flagForObj = true;
		if(flagForSettings == false){

			$('#wrapperForSettings').slideDown();	
			flagForSettings = true;
		}else{

			$('#wrapperForSettings').slideUp();	
			flagForSettings = false;
		}
		if(flagForTimezones == true){

			$('#divForTimezones').fadeOut();
			$('#supportDivForBottomBorder').fadeOut();
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});
	$('#getButton').on('click', function(){

		if(flagForSettings == true){

			$('#wrapperForSettings').slideUp();		
			flagForSettings = false;	
		}
		if(flagForTimezones == true){

			$('#divForTimezones').css('display', 'none');
			$('#supportDivForBottomBorder').css('display', 'none');
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}

		obj.name = document.getElementById("inputFIO").value;
		obj.dateStart = document.getElementById("datepickerStart").value;
		obj.dateFinish = document.getElementById("datepickerFinish").value;
		obj.timeZone = document.getElementById("timezone").value;

		obj.dateStart = obj.dateStart.charAt(6) + obj.dateStart.charAt(7) + obj.dateStart.charAt(8) + obj.dateStart.charAt(9) + '-' + obj.dateStart.charAt(0) + obj.dateStart.charAt(1) + '-' + obj.dateStart.charAt(3) + obj.dateStart.charAt(4);
		obj.dateFinish = obj.dateFinish.charAt(6) + obj.dateFinish.charAt(7) + obj.dateFinish.charAt(8) + obj.dateFinish.charAt(9) + '-' + obj.dateFinish.charAt(0) + obj.dateFinish.charAt(1) + '-' + obj.dateFinish.charAt(3) + obj.dateFinish.charAt(4);

		var srcForInput = document.location.href + 'schedule?name=' + obj.name + '&dateStart=' + obj.dateStart + '&dateFinish=' + obj.dateFinish + '&timezoneStart=' + obj.timeZone;
		console.log(srcForInput);

		$('#copyDiv').attr('value', srcForInput);
		console.log(obj);
		$('#wrapperForSecondModal').fadeIn();

	});

	$('#qestionForTimezone').on('click', function(){
		
		$('#timezone').attr('value', valueForTimezone);
	});

	$('#flexibleImage').on('click', function(){

		if(flagForDiscription == false){

			$('#wrapperForFirstModal').fadeIn('slow');
			$('#wrapperForFirstModal').css('overflow', 'hidden');
			flagForDiscription = true;
		}
	});
	$('#wrapperForFirstModal').on('click', function(){

		if(flagForDiscription == true){

			$('#wrapperForFirstModal').fadeOut();
			$('#wrapperForFirstModal').css('overflow', 'auto');
			flagForDiscription = false;
		}
	});
	$('#close').on('click', function(){

		$('#wrapperForSecondModal').fadeOut();
		$('#wrapperForSecondModal').css('overflow', 'auto');
	});

	$('#buttonForCopy').on('click', function(){

		$('#copyDiv').execCommand("copy");
	});


	document.getElementById('buttonForCopy').addEventListener('click', function(){

	    copyToClipboard(document.getElementById('copyDiv'));
	});

	function copyToClipboard(elem) {

	    var targetId = '_hiddenCopyText_';
	    var isInput = elem.tagName === 'INPUT' || elem.tagName === 'TEXTAREA';
	    var origSelectionStart, origSelectionEnd;

	    if (isInput) {
	    	
	        target = elem;
	        origSelectionStart = elem.selectionStart;
	        origSelectionEnd = elem.selectionEnd;
	    } else {

	        target = document.getElementById(targetId);
	        if (!target) {
	            var target = document.createElement('textarea');
	            target.style.position = 'absolute';
	            target.style.left = '-9999px';
	            target.style.top = '0';
	            target.id = targetId;
	            document.body.appendChild(target);
	        }
	        target.textContent = elem.textContent;
	    }

	    var currentFocus = document.activeElement;
	    target.focus();
	    target.setSelectionRange(0, target.value.length);
	    
	    var succeed;
	    try {
	    	  succeed = document.execCommand('copy');
	    } catch(e) {
	        succeed = false;
	    }

	    if (currentFocus && typeof currentFocus.focus === 'function') {
	        currentFocus.focus();
	    }
	    
	    if (isInput) {

	        elem.setSelectionRange(origSelectionStart, origSelectionEnd);
	    } else {

	        target.textContent = '';
	    }
	    return succeed;
	}


});