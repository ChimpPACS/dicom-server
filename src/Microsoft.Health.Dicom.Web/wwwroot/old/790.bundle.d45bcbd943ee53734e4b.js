"use strict";(globalThis.webpackChunk=globalThis.webpackChunk||[]).push([[790],{50511:(o,e,t)=>{t.r(e),t.d(e,{default:()=>x});var i=t(63677),n=t(22324);const{windowLevelPresets:l}=n.default;function a(o,e,t,i,n,l){return{id:e,icon:t,label:i,type:o,commands:n,tooltip:l}}const s=a.bind(null,"action"),c=(a.bind(null,"toggle"),a.bind(null,"tool"));function r(o,e,t){return{id:o.toString(),title:e,subtitle:t,type:"action",commands:[{commandName:"setWindowLevel",commandOptions:{...l[o]},context:"CORNERSTONE"}]}}const d=[{id:"MeasurementTools",type:"ohif.splitButton",props:{groupId:"MeasurementTools",isRadio:!0,primary:c("Length","tool-length","Length",[{commandName:"setToolActive",commandOptions:{toolName:"Length"},context:"CORNERSTONE"}],"Length"),secondary:{icon:"chevron-down",label:"",isActive:!0,tooltip:"More Measure Tools"},items:[c("Length","tool-length","Length",[{commandName:"setToolActive",commandOptions:{toolName:"Length"},context:"CORNERSTONE"}],"Length Tool"),c("Bidirectional","tool-bidirectional","Bidirectional",[{commandName:"setToolActive",commandOptions:{toolName:"Bidirectional"},context:"CORNERSTONE"}],"Bidirectional Tool"),c("EllipticalROI","tool-elipse","Ellipse",[{commandName:"setToolActive",commandOptions:{toolName:"EllipticalROI"},context:"CORNERSTONE"}],"Ellipse Tool"),c("CircleROI","tool-circle","Circle",[{commandName:"setToolActive",commandOptions:{toolName:"CircleROI"},context:"CORNERSTONE"}],"Circle Tool")]}},{id:"Zoom",type:"ohif.radioGroup",props:{type:"tool",icon:"tool-zoom",label:"Zoom",commands:[{commandName:"setToolActive",commandOptions:{toolName:"Zoom"},context:"CORNERSTONE"}]}},{id:"WindowLevel",type:"ohif.splitButton",props:{groupId:"WindowLevel",primary:c("WindowLevel","tool-window-level","Window Level",[{commandName:"setToolActive",commandOptions:{toolName:"WindowLevel"},context:"CORNERSTONE"}],"Window Level"),secondary:{icon:"chevron-down",label:"W/L Manual",isActive:!0,tooltip:"W/L Presets"},isAction:!0,renderer:i.eJ,items:[r(1,"Soft tissue","400 / 40"),r(2,"Lung","1500 / -600"),r(3,"Liver","150 / 90"),r(4,"Bone","2500 / 480"),r(5,"Brain","80 / 40")]}},{id:"Pan",type:"ohif.radioGroup",props:{type:"tool",icon:"tool-move",label:"Pan",commands:[{commandName:"setToolActive",commandOptions:{toolName:"Pan"},context:"CORNERSTONE"}]}},{id:"Capture",type:"ohif.action",props:{icon:"tool-capture",label:"Capture",type:"action",commands:[{commandName:"showDownloadViewportModal",commandOptions:{},context:"CORNERSTONE"}]}},{id:"Layout",type:"ohif.layoutSelector"},{id:"MoreTools",type:"ohif.splitButton",props:{isRadio:!0,groupId:"MoreTools",primary:s("Reset","tool-reset","Reset View",[{commandName:"resetViewport",commandOptions:{},context:"CORNERSTONE"}],"Reset"),secondary:{icon:"chevron-down",label:"",isActive:!0,tooltip:"More Tools"},items:[s("Reset","tool-reset","Reset View",[{commandName:"resetViewport",commandOptions:{},context:"CORNERSTONE"}],"Reset"),s("rotate-right","tool-rotate-right","Rotate Right",[{commandName:"rotateViewportCW",commandOptions:{},context:"CORNERSTONE"}],"Rotate +90"),s("flip-horizontal","tool-flip-horizontal","Flip Horizontally",[{commandName:"flipViewportHorizontal",commandOptions:{},context:"CORNERSTONE"}],"Flip Horizontal"),c("StackScroll","tool-stack-scroll","Stack Scroll",[{commandName:"setToolActive",commandOptions:{toolName:"StackScroll"},context:"CORNERSTONE"}],"Stack Scroll"),s("invert","tool-invert","Invert",[{commandName:"invertViewport",commandOptions:{},context:"CORNERSTONE"}],"Invert Colors"),c("CalibrationLine","tool-calibration","Calibration",[{commandName:"setToolActive",commandOptions:{toolName:"CalibrationLine"},context:"CORNERSTONE"}],"Calibration Line")]}}];var m=t(84334);const p=JSON.parse('{"u2":"@ohif/mode-basic-dev-mode"}').u2,u={Length:{}},N="@ohif/extension-default.layoutTemplateModule.viewerLayout",v="@ohif/extension-default.sopClassHandlerModule.stack",O="@ohif/extension-default.panelModule.measure",h="@ohif/extension-default.panelModule.seriesList",R="@ohif/extension-cornerstone.viewportModule.cornerstone",T="@ohif/extension-cornerstone-dicom-sr.sopClassHandlerModule.dicom-sr",E="@ohif/extension-dicom-video.sopClassHandlerModule.dicom-video",f="@ohif/extension-dicom-video.viewportModule.dicom-video",S="@ohif/extension-dicom-pdf.sopClassHandlerModule.dicom-pdf",g="@ohif/extension-dicom-pdf.viewportModule.dicom-pdf",y={"@ohif/extension-default":"^3.0.0","@ohif/extension-cornerstone":"^3.0.0","@ohif/extension-cornerstone-dicom-sr":"^3.0.0","@ohif/extension-dicom-pdf":"^3.0.1","@ohif/extension-dicom-video":"^3.0.1"};const x={id:p,modeFactory:function(o){let{modeConfiguration:e}=o;return{id:p,routeName:"dev",displayName:"Basic Dev Viewer",onModeEnter:o=>{let{servicesManager:e,extensionManager:t}=o;const{toolbarService:i,toolGroupService:n}=e.services,l=t.getModuleEntry("@ohif/extension-cornerstone.utilityModule.tools"),{toolNames:a,Enums:s}=l.exports,c={active:[{toolName:a.WindowLevel,bindings:[{mouseButton:s.MouseBindings.Primary}]},{toolName:a.Pan,bindings:[{mouseButton:s.MouseBindings.Auxiliary}]},{toolName:a.Zoom,bindings:[{mouseButton:s.MouseBindings.Secondary}]},{toolName:a.StackScrollMouseWheel,bindings:[]}],passive:[{toolName:a.Length},{toolName:a.Bidirectional},{toolName:a.Probe},{toolName:a.EllipticalROI},{toolName:a.CircleROI},{toolName:a.RectangleROI},{toolName:a.StackScroll},{toolName:a.CalibrationLine}]};let r;n.createToolGroupAndAddTools("default",c,u);({unsubscribe:r}=n.subscribe(n.EVENTS.VIEWPORT_ADDED,(()=>{i.recordInteraction({groupId:"WindowLevel",itemId:"WindowLevel",interactionType:"tool",commands:[{commandName:"setToolActive",commandOptions:{toolName:"WindowLevel"},context:"CORNERSTONE"}]}),r()}))),i.init(t),i.addButtons(d),i.createButtonSection("primary",["MeasurementTools","Zoom","WindowLevel","Pan","Layout","MoreTools"])},onModeExit:o=>{let{servicesManager:e}=o;const{toolGroupService:t,measurementService:i,toolbarService:n}=e.services;t.destroy()},validationTags:{study:[],series:[]},isValidMode:o=>{let{modalities:e}=o;return!e.split("\\").includes("SM")},routes:[{path:"viewer-cs3d",layoutTemplate:o=>{let{location:e,servicesManager:t}=o;return{id:N,props:{leftPanels:[h],rightPanels:[O],viewports:[{namespace:R,displaySetsToDisplay:[v]},{namespace:f,displaySetsToDisplay:[E]},{namespace:g,displaySetsToDisplay:[S]}]}}}}],extensions:y,hangingProtocol:"default",sopClassHandlers:[E,v,S,T],hotkeys:[...m.dD.defaults.hotkeyBindings]}},extensionDependencies:y}}}]);
//# sourceMappingURL=790.bundle.d45bcbd943ee53734e4b.js.map