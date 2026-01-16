
using Mediapipe.Unity.Sample;
using Mediapipe.Unity.Sample.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

  public class TestingConfig : ModalContents
  {
    private const string _ShowDebugPath = "Scroll View/Viewport/Contents/ShowDebug/Toggle";
    private const string _DistanceFilterPath = "Scroll View/Viewport/Contents/DistanceFilter/Toggle";
    private const string _AverageFilterPath = "Scroll View/Viewport/Contents/AverageFilter/Toggle";
  
    private Toggle _ShowDebugInput;
    private Toggle _DistanceFilterInput;
    private Toggle _AverageFilterInput;

    private bool _isChanged;

    private void Start()
    {
      InitializeContents();
    }

    public override void Exit()
    {
      GetModal().CloseAndResume(_isChanged);
    }

    private void InitializeContents()
    {
        InitializeShowDebug();
        InitializeUseDistance();
        InitializeAverageFilter();
    }

    private void InitializeShowDebug()
    {
        _ShowDebugInput = gameObject.transform.Find(_ShowDebugPath).gameObject.GetComponent<Toggle>();

      var imageSource = ImageSourceProvider.ImageSource;
        _ShowDebugInput.isOn = imageSource.isHorizontallyFlipped;
        _ShowDebugInput.onValueChanged.AddListener(delegate
      {
        imageSource.showDebug = _ShowDebugInput.isOn;
        _isChanged = true;
      });
    }

    private void InitializeUseDistance()
    {
        _DistanceFilterInput = gameObject.transform.Find(_DistanceFilterPath).gameObject.GetComponent<Toggle>();

        var imageSource = ImageSourceProvider.ImageSource;
        _DistanceFilterInput.isOn = imageSource.isHorizontallyFlipped;
        _DistanceFilterInput.onValueChanged.AddListener(delegate
        {
            imageSource.distanceFilter = _DistanceFilterInput.isOn;
            _isChanged = true;
        });
    }
    private void InitializeAverageFilter()
    {
        _AverageFilterInput = gameObject.transform.Find(_AverageFilterPath).gameObject.GetComponent<Toggle>();

        var imageSource = ImageSourceProvider.ImageSource;
        _AverageFilterInput.isOn = imageSource.isHorizontallyFlipped;
        _AverageFilterInput.onValueChanged.AddListener(delegate
        {
            imageSource.averageFilter = _AverageFilterInput.isOn;
            _isChanged = true;
        });
    }


}
