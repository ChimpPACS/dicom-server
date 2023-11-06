"use strict";(globalThis.webpackChunk=globalThis.webpackChunk||[]).push([[208],{9208:(o,e,t)=>{t.r(e),t.d(e,{default:()=>L});var n=t(84334),a=t(63677),i=t(22324);const{windowLevelPresets:l}=i.default;function s(o,e,t,n,a,i,l){return{id:e,icon:t,label:n,type:o,commands:a,tooltip:i,uiType:l}}const m=s.bind(null,"action"),c=s.bind(null,"toggle"),r=s.bind(null,"tool");function d(o,e,t){return{id:o.toString(),title:e,subtitle:t,type:"action",commands:[{commandName:"setWindowLevel",commandOptions:{...l[o]},context:"CORNERSTONE"}]}}const p=[{id:"MeasurementTools",type:"ohif.splitButton",props:{groupId:"MeasurementTools",isRadio:!0,primary:r("Length","tool-length","Length",[{commandName:"setToolActive",commandOptions:{toolName:"Length"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SRLength",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Length"),secondary:{icon:"chevron-down",label:"",isActive:!0,tooltip:"More Measure Tools"},items:[r("Length","tool-length","Length",[{commandName:"setToolActive",commandOptions:{toolName:"Length"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SRLength",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Length Tool"),r("Bidirectional","tool-bidirectional","Bidirectional",[{commandName:"setToolActive",commandOptions:{toolName:"Bidirectional"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SRBidirectional",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Bidirectional Tool"),r("ArrowAnnotate","tool-annotate","Annotation",[{commandName:"setToolActive",commandOptions:{toolName:"ArrowAnnotate"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SRArrowAnnotate",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Arrow Annotate"),r("EllipticalROI","tool-elipse","Ellipse",[{commandName:"setToolActive",commandOptions:{toolName:"EllipticalROI"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SREllipticalROI",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Ellipse Tool"),r("CircleROI","tool-circle","Circle",[{commandName:"setToolActive",commandOptions:{toolName:"CircleROI"},context:"CORNERSTONE"},{commandName:"setToolActive",commandOptions:{toolName:"SRCircleROI",toolGroupId:"SRToolGroup"},context:"CORNERSTONE"}],"Circle Tool")]}},{id:"Zoom",type:"ohif.radioGroup",props:{type:"tool",icon:"tool-zoom",label:"Zoom",commands:[{commandName:"setToolActive",commandOptions:{toolName:"Zoom"},context:"CORNERSTONE"}]}},{id:"WindowLevel",type:"ohif.splitButton",props:{groupId:"WindowLevel",primary:r("WindowLevel","tool-window-level","Window Level",[{commandName:"setToolActive",commandOptions:{toolName:"WindowLevel"},context:"CORNERSTONE"}],"Window Level"),secondary:{icon:"chevron-down",label:"W/L Manual",isActive:!0,tooltip:"W/L Presets"},isAction:!0,renderer:a.eJ,items:[d(1,"Soft tissue","400 / 40"),d(2,"Lung","1500 / -600"),d(3,"Liver","150 / 90"),d(4,"Bone","2500 / 480"),d(5,"Brain","80 / 40")]}},{id:"Pan",type:"ohif.radioGroup",props:{type:"tool",icon:"tool-move",label:"Pan",commands:[{commandName:"setToolActive",commandOptions:{toolName:"Pan"},context:"CORNERSTONE"}]}},{id:"Capture",type:"ohif.action",props:{icon:"tool-capture",label:"Capture",type:"action",commands:[{commandName:"showDownloadViewportModal",commandOptions:{},context:"CORNERSTONE"}]}},{id:"Layout",type:"ohif.splitButton",props:{groupId:"LayoutTools",isRadio:!1,primary:{id:"Layout",type:"action",uiType:"ohif.layoutSelector",icon:"tool-layout",label:"Grid Layout",props:{rows:4,columns:4,commands:[{commandName:"setLayout",commandOptions:{},context:"CORNERSTONE"}]}},secondary:{icon:"chevron-down",label:"",isActive:!0,tooltip:"Hanging Protocols"},items:[{id:"2x2",type:"action",label:"2x2",commands:[{commandName:"setHangingProtocol",commandOptions:{protocolId:"@ohif/mnGrid",stageId:"2x2"},context:"DEFAULT"}]},{id:"3x1",type:"action",label:"3x1",commands:[{commandName:"setHangingProtocol",commandOptions:{protocolId:"@ohif/mnGrid",stageId:"3x1"},context:"DEFAULT"}]},{id:"2x1",type:"action",label:"2x1",commands:[{commandName:"setHangingProtocol",commandOptions:{protocolId:"@ohif/mnGrid",stageId:"2x1"},context:"DEFAULT"}]},{id:"1x1",type:"action",label:"1x1",commands:[{commandName:"setHangingProtocol",commandOptions:{protocolId:"@ohif/mnGrid",stageId:"1x1"},context:"DEFAULT"}]}]}},{id:"MPR",type:"ohif.action",props:{type:"toggle",icon:"icon-mpr",label:"MPR",commands:[{commandName:"toggleHangingProtocol",commandOptions:{protocolId:"mpr"},context:"DEFAULT"}]}},{id:"Crosshairs",type:"ohif.radioGroup",props:{type:"tool",icon:"tool-crosshair",label:"Crosshairs",commands:[{commandName:"setToolActive",commandOptions:{toolGroupId:"mpr",toolName:"Crosshairs"},context:"CORNERSTONE"}]}},{id:"MoreTools",type:"ohif.splitButton",props:{isRadio:!0,groupId:"MoreTools",primary:m("Reset","tool-reset","Reset View",[{commandName:"resetViewport",commandOptions:{},context:"CORNERSTONE"}],"Reset"),secondary:{icon:"chevron-down",label:"",isActive:!0,tooltip:"More Tools"},items:[m("Reset","tool-reset","Reset View",[{commandName:"resetViewport",commandOptions:{},context:"CORNERSTONE"}],"Reset"),m("rotate-right","tool-rotate-right","Rotate Right",[{commandName:"rotateViewportCW",commandOptions:{},context:"CORNERSTONE"}],"Rotate +90"),m("flip-horizontal","tool-flip-horizontal","Flip Horizontally",[{commandName:"flipViewportHorizontal",commandOptions:{},context:"CORNERSTONE"}],"Flip Horizontal"),c("StackImageSync","link","Stack Image Sync",[{commandName:"toggleStackImageSync",commandOptions:{},context:"CORNERSTONE"}]),c("ReferenceLines","tool-referenceLines","Reference Lines",[{commandName:"toggleReferenceLines",commandOptions:{},context:"CORNERSTONE"}]),r("StackScroll","tool-stack-scroll","Stack Scroll",[{commandName:"setToolActive",commandOptions:{toolName:"StackScroll"},context:"CORNERSTONE"}],"Stack Scroll"),m("invert","tool-invert","Invert",[{commandName:"invertViewport",commandOptions:{},context:"CORNERSTONE"}],"Invert Colors"),r("Probe","tool-probe","Probe",[{commandName:"setToolActive",commandOptions:{toolName:"DragProbe"},context:"CORNERSTONE"}],"Probe"),c("cine","tool-cine","Cine",[{commandName:"toggleCine",context:"CORNERSTONE"}],"Cine"),r("Angle","tool-angle","Angle",[{commandName:"setToolActive",commandOptions:{toolName:"Angle"},context:"CORNERSTONE"}],"Angle"),r("Magnify","tool-magnify","Magnify",[{commandName:"setToolActive",commandOptions:{toolName:"Magnify"},context:"CORNERSTONE"}],"Magnify"),r("Rectangle","tool-rectangle","Rectangle",[{commandName:"setToolActive",commandOptions:{toolName:"RectangleROI"},context:"CORNERSTONE"}],"Rectangle"),m("TagBrowser","list-bullets","Dicom Tag Browser",[{commandName:"openDICOMTagViewer",commandOptions:{},context:"DEFAULT"}],"Dicom Tag Browser")]}}],u=JSON.parse('{"u2":"@ohif/mode-test"}').u2;const N=function(o,e,t){!function(o,e,t,n){const a=o.getModuleEntry("@ohif/extension-cornerstone.utilityModule.tools"),{toolNames:i,Enums:l}=a.exports,s={active:[{toolName:i.WindowLevel,bindings:[{mouseButton:l.MouseBindings.Primary}]},{toolName:i.Pan,bindings:[{mouseButton:l.MouseBindings.Auxiliary}]},{toolName:i.Zoom,bindings:[{mouseButton:l.MouseBindings.Secondary}]},{toolName:i.StackScrollMouseWheel,bindings:[]}],passive:[{toolName:i.Length},{toolName:i.ArrowAnnotate},{toolName:i.Bidirectional},{toolName:i.DragProbe},{toolName:i.EllipticalROI},{toolName:i.CircleROI},{toolName:i.RectangleROI},{toolName:i.StackScroll},{toolName:i.Angle},{toolName:i.Magnify},{toolName:i.SegmentationDisplay}],disabled:[{toolName:i.ReferenceLines}]},m={[i.ArrowAnnotate]:{getTextCallback:(o,e)=>t.runCommand("arrowTextCallback",{callback:o,eventDetails:e}),changeTextCallback:(o,e,n)=>t.runCommand("arrowTextCallback",{callback:n,data:o,eventDetails:e})}};e.createToolGroupAndAddTools(n,s,m)}(o,e,t,"default"),function(o,e,t){const n=o.getModuleEntry("@ohif/extension-cornerstone-dicom-sr.utilityModule.tools"),a=o.getModuleEntry("@ohif/extension-cornerstone.utilityModule.tools"),{toolNames:i}=n.exports,{toolNames:l,Enums:s}=a.exports,m={active:[{toolName:l.WindowLevel,bindings:[{mouseButton:s.MouseBindings.Primary}]},{toolName:l.Pan,bindings:[{mouseButton:s.MouseBindings.Auxiliary}]},{toolName:l.Zoom,bindings:[{mouseButton:s.MouseBindings.Secondary}]},{toolName:l.StackScrollMouseWheel,bindings:[]}],passive:[{toolName:i.SRLength},{toolName:i.SRArrowAnnotate},{toolName:i.SRBidirectional},{toolName:i.SREllipticalROI},{toolName:i.SRCircleROI}],enabled:[{toolName:i.DICOMSRDisplay,bindings:[]}]},c={[l.ArrowAnnotate]:{getTextCallback:(o,e)=>t.runCommand("arrowTextCallback",{callback:o,eventDetails:e}),changeTextCallback:(o,e,n)=>t.runCommand("arrowTextCallback",{callback:n,data:o,eventDetails:e})}};e.createToolGroupAndAddTools("SRToolGroup",m,c)}(o,e,t),function(o,e,t){const n=o.getModuleEntry("@ohif/extension-cornerstone.utilityModule.tools"),{toolNames:a,Enums:i}=n.exports,l={active:[{toolName:a.WindowLevel,bindings:[{mouseButton:i.MouseBindings.Primary}]},{toolName:a.Pan,bindings:[{mouseButton:i.MouseBindings.Auxiliary}]},{toolName:a.Zoom,bindings:[{mouseButton:i.MouseBindings.Secondary}]},{toolName:a.StackScrollMouseWheel,bindings:[]}],passive:[{toolName:a.Length},{toolName:a.ArrowAnnotate},{toolName:a.Bidirectional},{toolName:a.DragProbe},{toolName:a.EllipticalROI},{toolName:a.CircleROI},{toolName:a.RectangleROI},{toolName:a.StackScroll},{toolName:a.Angle},{toolName:a.SegmentationDisplay}],disabled:[{toolName:a.Crosshairs},{toolName:a.ReferenceLines}]},s={[a.Crosshairs]:{viewportIndicators:!1,autoPan:{enabled:!1,panSize:10}},[a.ArrowAnnotate]:{getTextCallback:(o,e)=>t.runCommand("arrowTextCallback",{callback:o,eventDetails:e}),changeTextCallback:(o,e,n)=>t.runCommand("arrowTextCallback",{callback:n,data:o,eventDetails:e})}};e.createToolGroupAndAddTools("mpr",l,s)}(o,e,t)},g=["SM","ECG","SR","SEG"],R="@ohif/extension-default.layoutTemplateModule.viewerLayout",O="@ohif/extension-default.sopClassHandlerModule.stack",T="@ohif/extension-measurement-tracking.panelModule.trackedMeasurements",S="@ohif/extension-measurement-tracking.panelModule.seriesList",x="@ohif/extension-measurement-tracking.viewportModule.cornerstone-tracked",y="@ohif/extension-cornerstone-dicom-sr.sopClassHandlerModule.dicom-sr",E="@ohif/extension-cornerstone-dicom-sr.viewportModule.dicom-sr",h="@ohif/extension-dicom-video.sopClassHandlerModule.dicom-video",v="@ohif/extension-dicom-video.viewportModule.dicom-video",C="@ohif/extension-dicom-pdf.sopClassHandlerModule.dicom-pdf",f="@ohif/extension-dicom-pdf.viewportModule.dicom-pdf",b="@ohif/extension-cornerstone-dicom-seg.sopClassHandlerModule.dicom-seg",A="@ohif/extension-cornerstone-dicom-seg.viewportModule.dicom-seg",M="@ohif/extension-cornerstone-dicom-seg.panelModule.panelSegmentation",w={"@ohif/extension-default":"^3.0.0","@ohif/extension-cornerstone":"^3.0.0","@ohif/extension-measurement-tracking":"^3.0.0","@ohif/extension-cornerstone-dicom-sr":"^3.0.0","@ohif/extension-cornerstone-dicom-seg":"^3.0.0","@ohif/extension-dicom-pdf":"^3.0.1","@ohif/extension-dicom-video":"^3.0.1","@ohif/extension-test":"^0.0.1"};const L={id:u,modeFactory:function(){return{id:u,routeName:"basic-test",displayName:"Basic Test Mode",onModeEnter:o=>{let{servicesManager:e,extensionManager:t,commandsManager:n}=o;const{measurementService:a,toolbarService:i,toolGroupService:l,customizationService:s}=e.services;let m;a.clearMeasurements(),N(t,l,n),s.addModeCustomizations(["@ohif/extension-test.customizationModule.custom-context-menu"]);({unsubscribe:m}=l.subscribe(l.EVENTS.VIEWPORT_ADDED,(()=>{i.recordInteraction({groupId:"WindowLevel",itemId:"WindowLevel",interactionType:"tool",commands:[{commandName:"setToolActive",commandOptions:{toolName:"WindowLevel"},context:"CORNERSTONE"}]}),m()}))),i.init(t),i.addButtons(p),i.createButtonSection("primary",["MeasurementTools","Zoom","WindowLevel","Pan","Capture","Layout","MPR","Crosshairs","MoreTools"])},onModeExit:o=>{let{servicesManager:e}=o;const{toolGroupService:t,syncGroupService:n,segmentationService:a,cornerstoneViewportService:i}=e.services;t.destroy(),n.destroy(),a.destroy(),i.destroy()},validationTags:{study:[],series:[]},isValidMode:function(o){let{modalities:e}=o;return!!e.split("\\").filter((o=>-1===g.indexOf(o))).length},routes:[{path:"basic-test",layoutTemplate:()=>({id:R,props:{leftPanels:[S],rightPanels:[M,T],viewports:[{namespace:x,displaySetsToDisplay:[O]},{namespace:E,displaySetsToDisplay:[y]},{namespace:v,displaySetsToDisplay:[h]},{namespace:f,displaySetsToDisplay:[C]},{namespace:A,displaySetsToDisplay:[b]}]}})}],extensions:w,hangingProtocol:"default",sopClassHandlers:[h,b,O,C,y],hotkeys:{name:"basic-test-hotkeys",hotkeys:[...n.dD.defaults.hotkeyBindings]}}},extensionDependencies:w}}}]);
//# sourceMappingURL=208.bundle.1dc30a17a29fbde3b753.js.map