using TaleWorlds.Core;
using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.GauntletUI.Data;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View.MissionViews;

namespace BattleSummary
{
    public class SubModule : MBSubModuleBase
    {
        
        protected override void OnSubModuleLoad()
        {
            base.OnSubModuleLoad();

        }

        public override void OnMissionBehaviorInitialize(Mission mission)
        {
            base.OnMissionBehaviorInitialize(mission);

            mission.AddMissionBehavior(new BattleSummaryMissionView());

        }
    }

    public class BattleSummaryMissionView : MissionView
    {
        //Declare the fields needed to add a new UI layer to the MissionView (battle view).
        //This will be the UI to keep track of the kill counts during battle.        
        GauntletLayer _layer; 
        IGauntletMovie _movie;  
        BattleSummaryVM _dataSource;
        public override void OnMissionScreenInitialize()
        {
            base.OnMissionScreenInitialize();

            //Set up the new layer
            _dataSource = new BattleSummaryVM(Mission);
            _layer = new GauntletLayer(1);
            _movie = _layer.LoadMovie("BattleSummaryHUD", _dataSource);
            MissionScreen.AddLayer(_layer);
        }
        public override void OnMissionScreenFinalize()
        {
            base.OnMissionScreenFinalize();

            //Clean up the layer that was added.
            MissionScreen.RemoveLayer(_layer);
            _layer = null;
            _movie = null;
            _dataSource = null;        
        }

        //Test method to see if the mission view is responsive. Every time Q is pressed, the count should be incremented.
        public override void OnMissionScreenTick(float dt)
        {
            base.OnMissionScreenTick(dt);

            if (Input.IsKeyReleased(TaleWorlds.InputSystem.InputKey.Q))
            {
                InformationManager.DisplayMessage(new InformationMessage("Q was pressed."));
            }
        }

        public override void OnMissionModeChange(MissionMode oldMissionMode, bool atStart)
        {
            base.OnMissionModeChange(oldMissionMode, atStart);
            _dataSource?.OnMissionModeChanged(Mission);
        }
    }

    public class BattleSummaryVM : ViewModel
    {
        Mission _mission;

        public BattleSummaryVM(Mission mission)
        {
            _mission = mission;

            OnMissionModeChanged(mission);
        }

        public void OnMissionModeChanged(Mission mission)
        {
            IsVisible = mission.Mode is MissionMode.Battle;
        }

        bool _isVisible;
        [DataSourceProperty]
        public bool IsVisible
        {
            get
            {
                return this._isVisible;
            }
            set
            {
                if (value != this._isVisible)
                {
                    this._isVisible = value;
                    base.OnPropertyChangedWithValue(value, "IsVisible");
                }
            }
        }
    }
}
