
app.preferences.rulerUnits = Units.PIXELS;
app.preferences.typeUnits = TypeUnits.PIXELS


sourcename = app.activeDocument.name;
	sourcepath = app.activeDocument.path;

exportname = app.activeDocument.name.replace(/\.[^\.]+$/, '')+"_export";
exportpath = app.activeDocument.path;
var idsave = charIDToTypeID( "save" );
var desc338 = new ActionDescriptor();
var idAs = charIDToTypeID( "As  " );
var desc339 = new ActionDescriptor();
var idmaximizeCompatibility = stringIDToTypeID( "maximizeCompatibility" );
desc339.putBoolean( idmaximizeCompatibility, true );
var idPhtthree = charIDToTypeID( "Pht3" );
desc338.putObject( idAs, idPhtthree, desc339 );
var idIn = charIDToTypeID( "In  " );
desc338.putPath( idIn, new File( exportpath+"/" + exportname+".psd" ) );
var idDocI = charIDToTypeID( "DocI" );
desc338.putInteger( idDocI, 871 );
var idLwCs = charIDToTypeID( "LwCs" );
desc338.putBoolean( idLwCs, true );
var idsaveStage = stringIDToTypeID( "saveStage" );
var idsaveStageType = stringIDToTypeID( "saveStageType" );
var idsaveSucceeded = stringIDToTypeID( "saveSucceeded" );
desc338.putEnumerated( idsaveStage, idsaveStageType, idsaveSucceeded );
executeAction( idsave, desc338, DialogModes.NO );
var doc = app.activeDocument;
var layers = getSelectedLayers().reverse(); //reorder for unity
var TransformArray = [];
var _JSONFileCreated = false;



main();
function main() {
	if (!documents.length) return;
	for (var i = 0; i < layers.length; i++) {
		var currentLayer = layers[i];
		doc.activeLayer = currentLayer;
		var index = i;		

		if(currentLayer.name.indexOf(".png")>-1) {
			currentLayer.name = currentLayer.name.replace(/_*.png/, "");
		}
		currentLayer.name = currentLayer.name.replace(/\r/gm, "_").replace(/( copy)|( копия)/, "").replace(/[^A-Za-zА-Яа-яЁё0-9]/g, "_").replace(/ /g, "_").replace(/_+/g, "_")  + ".png";
		currentLayer.name = currentLayer.name.replace(/_*.png/, ".png");
		
		var LayerName = currentLayer.name; // + "_" + index
		//cropImage();
		//if(currentLayer.kind == LayerKind.TEXT){
		//getTextFromLayer(currentLayer);
		//}
		getLayerTransform(TransformArray);
	}
	var docName = app.activeDocument.name.replace(/\.[^\.]+$/, '');
	var path = app.activeDocument.path;
	var file = new File(path + "/" + docName + "-layout.json"); //+ "-assets/"
	file.open("w", "TEXT", "????");
	$.os.search(/windows/i) != -1 ? file.lineFeed = 'windows' : file.lineFeed = 'macintosh';
	var JSONNodesNames = ["name", "src", "info", "sprites"];
	for (i in JSONNodesNames) {
		file.writeln(addJSONNode(JSONNodesNames[i]));
	}
	file.close();
	//activeDocument.close(SaveOptions.DONOTSAVECHANGES);
	//app.docRef.close(SaveOptions.DONOTSAVECHANGES);	
	//var sourcefileRef = File(sourcepath +"/"+sourcename);
	//alert (sourcepath +"/"+sourcename);
	//app.open(sourcefileRef);
	//app.docRef.open(path +"/"+sourcename);
	alert("Export complete!");
	
}
function angleFromMatrix(yy, xy)
{
    var toDegs = 180/Math.PI;
    return Math.atan2(yy, xy) * toDegs - 90;
}

function getActiveLayerRotation()
{
    var ref = new ActionReference();
    ref.putEnumerated( charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt") );
    var desc = executeActionGet(ref).getObjectValue(stringIDToTypeID('textKey'))
    if (desc.hasKey(stringIDToTypeID('transform')))
    {
        desc = desc.getObjectValue(stringIDToTypeID('transform'))
        var yy = desc.getDouble(stringIDToTypeID('yy'));
        var xy = desc.getDouble(stringIDToTypeID('xy'));
        return angleFromMatrix(yy, xy);
    }
    return 0;
}
function selectLayerMask() {
	try{ 
		var id759 = charIDToTypeID( "slct" );
		var desc153 = new ActionDescriptor();
		var id760 = charIDToTypeID( "null" );
		var ref92 = new ActionReference();
		var id761 = charIDToTypeID( "Chnl" );
		var id762 = charIDToTypeID( "Chnl" );
		var id763 = charIDToTypeID( "Msk " );
		ref92.putEnumerated( id761, id762, id763 );
		desc153.putReference( id760, ref92 );
		var id764 = charIDToTypeID( "MkVs" );
		desc153.putBoolean( id764, false );
		executeAction( id759, desc153, DialogModes.NO );
	}catch(e) {
		
	}
}
function rasterizeLayer() {
	try{ 
		var id1242 = stringIDToTypeID( "rasterizeLayer" );
		var desc245 = new ActionDescriptor();
		var id1243 = charIDToTypeID( "null" );
		var ref184 = new ActionReference();
		var id1244 = charIDToTypeID( "Lyr " );
		var id1245 = charIDToTypeID( "Ordn" );
		var id1246 = charIDToTypeID( "Trgt" );
		ref184.putEnumerated( id1244, id1245, id1246 );
		desc245.putReference( id1243, ref184 );
		executeAction( id1242, desc245, DialogModes.NO );
	}catch(e) {
		
	}
}
function deleteLayerMask(apply) {
	var c2t = function (s) {
		return app.charIDToTypeID(s);
	};
	var s2t = function (s) {
		return app.stringIDToTypeID(s);
	};
	var descriptor = new ActionDescriptor();
	var reference = new ActionReference();
	reference.putEnumerated( s2t( "channel" ), s2t( "channel" ), s2t( "mask" ));
	descriptor.putReference( c2t( "null" ), reference );
	descriptor.putBoolean( s2t( "apply" ), apply );
	executeAction( s2t( "delete" ), descriptor, DialogModes.NO );
}
function ConvertToPoint() {	
	var idsetd = charIDToTypeID( "setd" );
	var desc1343 = new ActionDescriptor();
	var idnull = charIDToTypeID( "null" );
	var ref348 = new ActionReference();
	var idPrpr = charIDToTypeID( "Prpr" );
	var idTEXT = charIDToTypeID( "TEXT" );
	ref348.putProperty( idPrpr, idTEXT );
	var idTxLr = charIDToTypeID( "TxLr" );
	var idOrdn = charIDToTypeID( "Ordn" );
	var idTrgt = charIDToTypeID( "Trgt" );
	ref348.putEnumerated( idTxLr, idOrdn, idTrgt );
	desc1343.putReference( idnull, ref348 );
	var idT = charIDToTypeID( "T   " );
	var idTEXT = charIDToTypeID( "TEXT" );
	var idPnt = charIDToTypeID( "Pnt " );
	desc1343.putEnumerated( idT, idTEXT, idPnt );
	executeAction( idsetd, desc1343, DialogModes.NO );
}
function ConvertToParagraph() {	
	var idsetd = charIDToTypeID( "setd" );
	var desc992 = new ActionDescriptor();
	var idnull = charIDToTypeID( "null" );
	var ref345 = new ActionReference();
	var idPrpr = charIDToTypeID( "Prpr" );
	var idTEXT = charIDToTypeID( "TEXT" );
	ref345.putProperty( idPrpr, idTEXT );
	var idTxLr = charIDToTypeID( "TxLr" );
	var idOrdn = charIDToTypeID( "Ordn" );
	var idTrgt = charIDToTypeID( "Trgt" );
	ref345.putEnumerated( idTxLr, idOrdn, idTrgt );
	desc992.putReference( idnull, ref345 );
	var idT = charIDToTypeID( "T   " );
	var idTEXT = charIDToTypeID( "TEXT" );
	var idbox = stringIDToTypeID( "box" );
	desc992.putEnumerated( idT, idTEXT, idbox );
	executeAction( idsetd, desc992, DialogModes.NO );
}	
function mergeGroup() {	
	var idMrgtwo = charIDToTypeID( "Mrg2" );
	executeAction( idMrgtwo, undefined, DialogModes.NO );
}
function setColorOfFillLayer( sColor ) {
    var desc = new ActionDescriptor();
        var ref = new ActionReference();
        ref.putEnumerated( stringIDToTypeID('contentLayer'), charIDToTypeID('Ordn'), charIDToTypeID('Trgt') );
    desc.putReference( charIDToTypeID('null'), ref );
        var fillDesc = new ActionDescriptor();
            var colorDesc = new ActionDescriptor();
            colorDesc.putDouble( charIDToTypeID('Rd  '), sColor.rgb.red );
            colorDesc.putDouble( charIDToTypeID('Grn '), sColor.rgb.green );
            colorDesc.putDouble( charIDToTypeID('Bl  '), sColor.rgb.blue );
        fillDesc.putObject( charIDToTypeID('Clr '), charIDToTypeID('RGBC'), colorDesc );
    desc.putObject( charIDToTypeID('T   '), stringIDToTypeID('solidColorLayer'), fillDesc );
    executeAction( charIDToTypeID('setd'), desc, DialogModes.NO );
}
function getFillColor(){
var ref = new ActionReference();
   ref.putEnumerated( stringIDToTypeID( "contentLayer" ), charIDToTypeID( "Ordn" ), charIDToTypeID( "Trgt" ));
var ref1= executeActionGet( ref );
var list =  ref1.getList( charIDToTypeID( "Adjs" ) ) ;
var solidColorLayer = list.getObjectValue(0);        
var color = solidColorLayer.getObjectValue(charIDToTypeID('Clr ')); 
var fillcolor = new SolidColor;
   fillcolor.rgb.red = color.getDouble(charIDToTypeID('Rd  '));
   fillcolor.rgb.green = color.getDouble(charIDToTypeID('Grn '));
   fillcolor.rgb.blue = color.getDouble(charIDToTypeID('Bl  '));
return fillcolor;
}

function getLayerTransform(arr) {
	var layer = activeDocument.activeLayer;
	var lName = layer.name;
	
		var isFolder = (layer instanceof LayerSet);
		var layerParent = layer.parent.name;
		var groupParent = layer.parent.parent.name;
		//alert (groupParent+" > "+layerParent+" > "+layer.name);
		//if ( layerParent !== doc ) doc.activeLayer = layerParent;
	
	type = (layer.kind  == LayerKind.TEXT)?"text":"image";
	
	if (layer.layers && layer.layers.length>0){
		//layer.merge();
		mergeGroup();
		layer = activeDocument.activeLayer;
	}
	if (!type){
		rasterizeLayer();
	}
	if(hasLayerMask() == true){
		selectLayerMask();
		deleteLayerMask(true);
	}
	var fillColor = "cc0000";
	if (hasVectorMask() == true){
		//selectLayerMask();
		//deleteLayerMask(true);
		type = "vectorMask";
		var fillColor = getFillColor().rgb.hexValue;
		//alert ("hasVectorMask: "+lName+"\n color = "+fillColor);
		
		var WhiteColor = new SolidColor();  
        WhiteColor.rgb.red = 255;  
        WhiteColor.rgb.green = 255;  
        WhiteColor.rgb.blue = 255;  
        //activeDocument.selection.fill( WhiteColor); 
		
		setColorOfFillLayer( WhiteColor );
		//getSolidColor();
		//getOpacity();
	}
	if(hasFilterMask()){
		//alert ("hasFilterMask: "+lName);
	}
	
	var x = layer.bounds[0].value;
	var y = layer.bounds[1].value;
	var width = layer.bounds[2] - layer.bounds[0];
	var height = layer.bounds[3] - layer.bounds[1];
	width = width.toString().replace(' px', '');
	height = height.toString().replace(' px', '');
	var opacity = layer.opacity;
	var blendMode = layer.blendMode;
	
	var attrString = "";
	if(type == "text"){
		if(layer.textItem && layer.textItem.kind == TextType.PARAGRAPHTEXT){
			
		} else {
			ConvertToParagraph();
		}
		
		var textalignment = "Justification.LEFT";
		if (layer.textItem.justification != null){
			var textalignment = layer.textItem.justification;
		}		
		var textrotation = getActiveLayerRotation();
		
		var textitalic = "";
		//if(layer.textItem.fauxItalic){
		//textitalic = layer.textItem.fauxItalic;
		//}		
		
		
		var artLayerRef = layer;
		var newLayer = artLayerRef.duplicate();
		newLayer.rotate(textrotation);
		newLayer.rasterize(RasterizeType.ENTIRELAYER);
		var width = newLayer.bounds[2] - newLayer.bounds[0];
		var height = newLayer.bounds[3] - newLayer.bounds[1];
		
		//var x = newLayer.bounds[0];
		var y = newLayer.bounds[1];
		
		newLayer.remove();	
		var x = layer.textItem.position[0];
		//var y = layer.textItem.position[1];	
		
		var width = layer.textItem.width*getCoeffSizeFromLayer(layer);
		//var height = layer.textItem.height*getCoeffSizeFromLayer(layer);
		
		x = x.toString().replace(' px', '');
		y = y.toString().replace(' px', '');
		width = width.toString().replace(' px', '');
		height = height.toString().replace(' px', '');
		
		var textSize = layer.textItem.size*getCoeffSizeFromLayer(layer);
		textSize = textSize.toString().replace(' px', '');
		var textFont = layer.textItem.font;	
		
		var textColor = layer.textItem.color.rgb.hexValue;
		var Contents = layer.textItem.contents;
		Contents = Contents.replace(/\r/g, "\\n");		
		
		//attrString += '[';
		attrString += '\n' + '\t\t\t\t{';
		attrString += '\n' + '\t\t\t\t\t"font":' + '"' + textFont + '",';
		attrString += '\n' + '\t\t\t\t\t"size":' + '' + textSize + ',';
		attrString += '\n' + '\t\t\t\t\t"color":' + '"' +"#"+ textColor + '",';
		attrString += '\n' + '\t\t\t\t\t"alignment":' + '"' + textalignment + '",';
		attrString += '\n' + '\t\t\t\t\t"rotation":' + '' + textrotation + ',';
		attrString += '\n' + '\t\t\t\t\t"italic":' + '"' + textitalic + '",';
		attrString += '\n' + '\t\t\t\t\t"contents":' + '"' + Contents + '"';
		attrString += '\n' + '\t\t\t\t}';
		//attrString += '\n' +  '\t\t\t]';
		
	} else{
		attrString = 0;
	}
	arr.push([
	[lName],
	[x],
	[y],
	[width],
	[height],
	[opacity],
	[fillColor],
	[blendMode],
	[type],
	[attrString],
	[layerParent],
	[groupParent]
	]);
	
	return (arr);
}
function getCoeffSizeFromLayer(CurrLayer) {
	var textSize = CurrLayer.textItem.size;
	
	var size = CurrLayer.textItem.size;
	var r = new ActionReference();	
	r.putProperty(stringIDToTypeID("property"), stringIDToTypeID("textKey"));
	r.putEnumerated(stringIDToTypeID("layer"), stringIDToTypeID("ordinal"), stringIDToTypeID("targetEnum"));
	var yy = 1;
	var yx = 0;
	try {
		var transform = executeActionGet(r).getObjectValue(stringIDToTypeID("textKey")).getObjectValue(stringIDToTypeID("transform"));
		yy = transform.getDouble(stringIDToTypeID("yy"));
		yx = transform.getDouble(stringIDToTypeID("yx"));
	}
	catch(e) { }
	var coeff = Math.sqrt(yy*yy + yx*yx);
	//coeff = 1;
	//textSize = size*coeff;
	return(coeff);
}

function cropImage(){
	doc = app.activeDocument;
	var bounds, left, top, right, bottom;
	left = 0;
	top = 0;
	right = doc.width;
	bottom = doc.height;
	bounds = [left, top, right, bottom];
	doc.crop(bounds);
}
function addJSONNode(typ, fil) {
	if (!_JSONFileCreated) {
	}	
	_JSONFileCreated = true;
	var tempStr = "";
	switch (typ) {
	case "name":
		tempStr += '{';
		tempStr += '\n\t"name":' + '"' + app.activeDocument.name + '",';
		return (tempStr);
		break;
	case "src":
		tempStr += '\t"src":' + '"' + app.activeDocument.path + '",';
		return (tempStr);
		break;
	case "info":
		tempStr += '\t"info":' + '{\n' + '\t"date":' + '"' + new Date().toString() + '"' + '\n\t},';
		return (tempStr);
		break;
	case "sprites":
		tempStr += '\t"sprites":[';
		var arrStr = "";
		for (var inf = 0; inf < TransformArray.length; inf++) {
			arrStr += '\n' + '\t\t{';
			arrStr += '\n' + '\t\t\t"name":' + '"' + TransformArray[inf][0] + '",';
			arrStr += '\n' + '\t\t\t"x":' + TransformArray[inf][1] + ',';
			arrStr += '\n' + '\t\t\t"y":' + TransformArray[inf][2] + ',';
			arrStr += '\n' + '\t\t\t"width":' + TransformArray[inf][3] + ',';
			arrStr += '\n' + '\t\t\t"height":' + '' + TransformArray[inf][4] + ',';
			arrStr += '\n' + '\t\t\t"opacity":' + '' + TransformArray[inf][5] + ',';
			arrStr += '\n' + '\t\t\t"fillColor":' + '"' + "#"+TransformArray[inf][6] + '",';
			arrStr += '\n' + '\t\t\t"blendMode":' + '"' + TransformArray[inf][7] + '",';
			arrStr += '\n' + '\t\t\t"type":' + '"' + TransformArray[inf][8] + '",';
			arrStr += '\n' + '\t\t\t"textContents":' + TransformArray[inf][9] + ',';
			arrStr += '\n' + '\t\t\t"layerParent":' + '"' +  TransformArray[inf][10] + '",';
			arrStr += '\n' + '\t\t\t"groupParent":' + '"' +  TransformArray[inf][11] + '"';
			arrStr += '\n' + '\t\t}';
			if (inf != TransformArray.length - 1) arrStr += ',';
		}
		arrStr += '\n' + '\t]';
		tempStr += arrStr;
		tempStr += '\n}';
		return (tempStr);
		break;
	default:
	}
}
function getSelectedLayers() {
	var idGrp = stringIDToTypeID("groupLayersEvent");
	var descGrp = new ActionDescriptor();
	var refGrp = new ActionReference();
	refGrp.putEnumerated(charIDToTypeID("Lyr "), charIDToTypeID("Ordn"), charIDToTypeID("Trgt"));
	descGrp.putReference(charIDToTypeID("null"), refGrp);
	executeAction(idGrp, descGrp, DialogModes.ALL);
	var resultLayers = new Array();
	for (var ix = 0; ix < app.activeDocument.activeLayer.layers.length; ix++) {
		resultLayers.push(app.activeDocument.activeLayer.layers[ix]);
	}
	var id8 = charIDToTypeID("slct");
	var desc5 = new ActionDescriptor();
	var id9 = charIDToTypeID("null");
	var ref2 = new ActionReference();
	var id10 = charIDToTypeID("HstS");
	var id11 = charIDToTypeID("Ordn");
	var id12 = charIDToTypeID("Prvs");
	ref2.putEnumerated(id10, id11, id12);
	desc5.putReference(id9, ref2);
	executeAction(id8, desc5, DialogModes.NO);
	return resultLayers;
}
function hasLayerMask() { 
	var hasLayerMask = false; 
	try { 
		var ref = new ActionReference(); 
		var keyUserMaskEnabled = app.charIDToTypeID( 'UsrM' ); 
		ref.putProperty( app.charIDToTypeID( 'Prpr' ), keyUserMaskEnabled ); 
		ref.putEnumerated( app.charIDToTypeID( 'Lyr ' ), app.charIDToTypeID( 'Ordn' ), app.charIDToTypeID( 'Trgt' ) ); 
		var desc = executeActionGet( ref ); 
		if ( desc.hasKey( keyUserMaskEnabled ) ) { 
			hasLayerMask = true; 
		} 
	}catch(e) { 
		hasLayerMask = false; 
	} 
	return hasLayerMask; 
} 
function hasVectorMask() {
	var hasVectorMask = false;
	try {
		var ref = new ActionReference();
		var keyVectorMaskEnabled = app.stringIDToTypeID( 'vectorMask' );
		var keyKind = app.charIDToTypeID( 'Knd ' );
		ref.putEnumerated( app.charIDToTypeID( 'Path' ), app.charIDToTypeID( 'Ordn' ), keyVectorMaskEnabled );
		var desc = executeActionGet( ref );
		if ( desc.hasKey( keyKind ) ) {
			var kindValue = desc.getEnumerationValue( keyKind );
			if (kindValue == keyVectorMaskEnabled) {
				hasVectorMask = true;
			}
		}
	}catch(e) {
		hasVectorMask = false;
	}
	return hasVectorMask;
}
function hasFilterMask() { 
	var hasFilterMask = false; 
	try { 
		var ref = new ActionReference(); 
		var keyFilterMask = app.stringIDToTypeID("hasFilterMask"); 
		ref.putProperty( app.charIDToTypeID( 'Prpr' ), keyFilterMask); 
		ref.putEnumerated( app.charIDToTypeID( 'Lyr ' ), app.charIDToTypeID( 'Ordn' ), app.charIDToTypeID( 'Trgt' ) ); 
		var desc = executeActionGet( ref ); 
		if ( desc.hasKey( keyFilterMask ) && desc.getBoolean( keyFilterMask )) { 
			hasFilterMask = true; 
		} 
	}catch(e) { 
		hasFilterMask = false; 
	} 
	return hasFilterMask; 
}