const rootUrl = `${window.location.protocol}//${window.location.hostname}:${window.location.port}`;
const wadoRootUrl = `${rootUrl}/v1`;
const killEvent = e => {
  e.preventDefault && e.preventDefault();
  e.stopPropagation && e.stopPropagation();
  e.stopImmediatePropagation && e.stopImmediatePropagation();
};

window.config = {
  routerBasename: '/',
  whiteLabeling: {
    createLogoComponentFn: function(React) {
      var fsBtn = React.createElement(
        'button',
        {
          type: 'button',
          className: 'fs-tb-button',
          onClick: e =>
            killEvent(e) & document.getElementById('root').requestFullscreen(),
        },
        ['Fullscreen']
      );

      var ufsBtn = React.createElement(
        'button',
        {
          type: 'button',
          className: 'ufs-tb-button',
          onClick: e => killEvent(e) & document.exitFullscreen(),
        },
        ['Exit Fullscreen']
      );

      return React.createElement(
        'div',
        {
          style: {
            backgroundImage: 'url(assets/cplogo.png)',
            width: '200px',
            height: '38px',
            display: 'inline-block',
            backgroundSize: '200px',
            backgroundRepeat: 'no-repeat',
            position: 'relative',
          },
        },
        [fsBtn, ufsBtn]
      );
    },
  },
  extensions: [],
  modes: [],
  showStudyList: true,
  maxNumberOfWebWorkers: 3,
  omitQuotationForMultipartRequest: true,
  showWarningMessageForCrossOrigin: true,
  showCPUFallbackMessage: true,
  showLoadingIndicator: true,
  strictZSpacingForVolumeViewport: true,
  maxNumRequests: {
    interaction: 100,
    thumbnail: 75,
    prefetch: 25,
  },
  defaultDataSourceName: 'dicomweb',
  dataSources: [
    {
      friendlyName: 'dcmjs DICOMWeb Server',
      namespace: '@ohif/extension-default.dataSourcesModule.dicomweb',
      sourceName: 'dicomweb',
      configuration: {
        name: 'dicomweb',
        wadoUriRoot: wadoRootUrl,
        qidoRoot: wadoRootUrl,
        wadoRoot: wadoRootUrl,
        qidoSupportsIncludeField: false,
        supportsReject: false,
        imageRendering: 'wadors',
        thumbnailRendering: 'wadors',
        enableStudyLazyLoad: true,
        supportsFuzzyMatching: false,
        supportsWildcard: true,
        staticWado: true,
        singlepart: 'bulkdata,video',
        bulkDataURI: {
          enabled: true,
          relativeResolution: 'studies',
        },
      },
    },
    //{
    //    friendlyName: 'dicomweb delegating proxy',
    //    namespace: '@ohif/extension-default.dataSourcesModule.dicomwebproxy',
    //    sourceName: 'dicomwebproxy',
    //    configuration: {
    //        name: 'dicomwebproxy',
    //    },
    //},
    {
      friendlyName: 'dicom json',
      namespace: '@ohif/extension-default.dataSourcesModule.dicomjson',
      sourceName: 'dicomjson',
      configuration: {
        name: 'json',
      },
    },
    {
      friendlyName: 'dicom local',
      namespace: '@ohif/extension-default.dataSourcesModule.dicomlocal',
      sourceName: 'dicomlocal',
      configuration: {},
    },
  ],
  httpErrorHandler: e => {
    console.warn(e.status), console.warn('test, navigate to https://ohif.org/');
  },
  hotkeys: [
    {
      commandName: 'incrementActiveViewport',
      label: 'Next Viewport',
      keys: ['right'],
    },
    {
      commandName: 'decrementActiveViewport',
      label: 'Previous Viewport',
      keys: ['left'],
    },
    {
      commandName: 'rotateViewportCW',
      label: 'Rotate Right',
      keys: ['r'],
    },
    {
      commandName: 'rotateViewportCCW',
      label: 'Rotate Left',
      keys: ['l'],
    },
    {
      commandName: 'invertViewport',
      label: 'Invert',
      keys: ['i'],
    },
    {
      commandName: 'flipViewportHorizontal',
      label: 'Flip Horizontally',
      keys: ['h'],
    },
    {
      commandName: 'flipViewportVertical',
      label: 'Flip Vertically',
      keys: ['v'],
    },
    {
      commandName: 'scaleUpViewport',
      label: 'Zoom In',
      keys: ['+'],
    },
    {
      commandName: 'scaleDownViewport',
      label: 'Zoom Out',
      keys: ['-'],
    },
    {
      commandName: 'fitViewportToWindow',
      label: 'Zoom to Fit',
      keys: ['='],
    },
    {
      commandName: 'resetViewport',
      label: 'Reset',
      keys: ['space'],
    },
    {
      commandName: 'nextImage',
      label: 'Next Image',
      keys: ['down'],
    },
    {
      commandName: 'previousImage',
      label: 'Previous Image',
      keys: ['up'],
    },
    {
      commandName: 'setToolActive',
      commandOptions: {
        toolName: 'Zoom',
      },
      label: 'Zoom',
      keys: ['z'],
    },
    {
      commandName: 'windowLevelPreset1',
      label: 'W/L Preset 1',
      keys: ['1'],
    },
    {
      commandName: 'windowLevelPreset2',
      label: 'W/L Preset 2',
      keys: ['2'],
    },
    {
      commandName: 'windowLevelPreset3',
      label: 'W/L Preset 3',
      keys: ['3'],
    },
    {
      commandName: 'windowLevelPreset4',
      label: 'W/L Preset 4',
      keys: ['4'],
    },
    {
      commandName: 'windowLevelPreset5',
      label: 'W/L Preset 5',
      keys: ['5'],
    },
    {
      commandName: 'windowLevelPreset6',
      label: 'W/L Preset 6',
      keys: ['6'],
    },
    {
      commandName: 'windowLevelPreset7',
      label: 'W/L Preset 7',
      keys: ['7'],
    },
    {
      commandName: 'windowLevelPreset8',
      label: 'W/L Preset 8',
      keys: ['8'],
    },
    {
      commandName: 'windowLevelPreset9',
      label: 'W/L Preset 9',
      keys: ['9'],
    },
  ],
};
