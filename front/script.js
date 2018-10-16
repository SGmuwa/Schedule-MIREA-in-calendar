$('document').ready(function(){

	var flagForSettings = false;
	var flagForTimezones = false;
	var valueForTimezone = 'cool';
	var windowHeight = window.innerHeight;
	var height = ((window.innerHeight / 100) * 139.8) + 'px';
	var zonesArr = ['GMT Среднее время по Гринвичу','UTC Всемирное координированное время','WET Западноевропейское время','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','','',''];

	for(var i = 0; i < 43; i++){

		$('<div/>', {

			class: 'timezones',
			id: 'zone' + i,
		}).appendTo('#divForTimezones');
		document.getElementById('zone' + i).innerHTML += zonesArr[i];
	}

	$('#timezone').on('click', function(){
		
		if(flagForTimezones == false){

			$('#divForTimezones').css('display', 'block');
			$('#supportDivForBottomBorder').css('display', 'block');
			$('body').css('height', height);
			flagForTimezones = true;
		}else{

			$('#divForTimezones').css('display', 'none');
			$('#supportDivForBottomBorder').css('display', 'none');
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});

	$('.timezones').on('click', function(){

		var id = $(this).attr('id');
		id = id.replace('zone', '');
		$('#timezone').attr('value', zonesArr[id]);
		if(flagForTimezones == true){

			$('#divForTimezones').css('display', 'none');
			$('#supportDivForBottomBorder').css('display', 'none');
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});

	$('.fa-cog').on('click', function(){
		if(flagForSettings == false){

			$('#wrapperForSettings').css('display', 'block');	
			flagForSettings = true;
		}else{

			$('#wrapperForSettings').css('display', 'none');	
			flagForSettings = false;
		}
		if(flagForTimezones == true){

			$('#divForTimezones').css('display', 'none');
			$('#supportDivForBottomBorder').css('display', 'none');
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});
	$('#getButton').on('click', function(){

		if(flagForSettings == true){

			$('#wrapperForSettings').css('display', 'none');	
			flagForSettings = false;	
		}
		if(flagForTimezones == true){

			$('#divForTimezones').css('display', 'none');
			$('#supportDivForBottomBorder').css('display', 'none');
			$('body').css('height', windowHeight);
			flagForTimezones = false;
		}
	});
	$('#qestionForTimezone').on('click', function(){
		
		$('#timezone').attr('value', valueForTimezone);
	});


});