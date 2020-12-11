/*Export Table Init*/

"use strict"; 

$(document).ready(function() {
	$('#example1').DataTable( {
		dom: 'Bfrtip',
		buttons: [
			'copy', 'csv', 'excel', 'pdf', 'print'
		]
	} );
} );