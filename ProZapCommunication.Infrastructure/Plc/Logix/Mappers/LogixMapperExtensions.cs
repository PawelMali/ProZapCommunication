using ProZapCommunication.Core.Models.StationData.Read;
using ProZapCommunication.Core.Models.StationData.Results;
using ProZapCommunication.Core.Models.StationData.Write;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Read;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Results;
using ProZapCommunication.Infrastructure.Plc.Logix.Structures.StationData.Write;

namespace ProZapCommunication.Infrastructure.Plc.Logix.Mappers;

public static class LogixMapperExtensions
{
    // Rzutowanie struktury READ na model Core
    public static StationReadBaseModel ToCoreModel(this LogixStationReadBase plcData)
    {
        if (plcData == null) return null;

        return new StationReadBaseModel
        {
            Life = plcData.Life,
            TaskCounter = plcData.Task_Counter,
            TaskSendToPc = plcData.Task_Send_To_PC,
            TaskConfirmToPc = plcData.Task_Confirm_To_PC,
            TaskSuccessCounter = plcData.Task_Succes_Counter,
            TaskStatus = plcData.Task_Status,
            DMC = plcData.DMC.ToString(),
            DMCCamera = plcData.DMC_Camera.ToString(),
            CameraRequest = plcData.CameraRequest
        };
    }

    // Rzutowanie modelu Core na strukturę WRITE (przygotowanie do wysyłki do PLC)
    public static LogixStationWriteBase ToPlcStructure(this StationWriteBaseModel coreModel)
    {
        if (coreModel == null) return null;

        return new LogixStationWriteBase
        {
            Life = (short)coreModel.Life,
            Task_Counter = (ushort)coreModel.TaskCounter,
            // Tutaj mapujemy parametry sterujące wysyłane z PC:
            Task_Recived_From_PC = (short)coreModel.TaskSendToPLC,
            Task_Confirm_From_PC = (short)coreModel.TaskConfirmToPLC,
            Task_Succes_Counter = (ushort)coreModel.TaskSuccessCounter,
            Task_Status = (short)coreModel.TaskStatus,
            Camera_Found = coreModel.CameraFound,
            Camera_Moved = coreModel.CameraMoved
        };
    }

    #region Stacja 1

    public static Station1ResultsData ToCoreModel(this LogixStation1ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station1ResultsData
        {
            SN = plcData.SN.ToString(),
            SN_Status = plcData.SN_Status,
            Radiator_SN = plcData.Radiator_SN.ToString(),
            Radiator_Status = plcData.Radiator_Status,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,
            Grommet_Left_Bottom_Presence_Cam_1_Status = plcData.Grommet_Left_Bottom_Presence_Cam_1_Status,
            Grommet_Left_Bottom_Presence_Cam_1_Counter = plcData.Grommet_Left_Bottom_Presence_Cam_1_Counter,
            Grommet_Right_Bottom_Presence_Cam_2_Status = plcData.Grommet_Right_Bottom_Presence_Cam_2_Status,
            Grommet_Right_Bottom_Presence_Cam_2_Counter = plcData.Grommet_Right_Bottom_Presence_Cam_2_Counter,
            Radiator_Left_Bottom_Grommet_Cam_1_Status = plcData.Radiator_Left_Bottom_Grommet_Cam_1_Status,
            Radiator_Left_Bottom_Grommet_Cam_1_Counter = plcData.Radiator_Left_Bottom_Grommet_Cam_1_Counter,
            Radiator_Right_Bottom_Grommet_Cam_2_Status = plcData.Radiator_Right_Bottom_Grommet_Cam_2_Status,
            Radiator_Right_Bottom_Grommet_Cam_2_Counter = plcData.Radiator_Right_Bottom_Grommet_Cam_2_Counter,
            Stonegrid_Bottom_Left_Bracket_Cam_3_Status = plcData.Stonegrid_Bottom_Left_Bracket_Cam_3_Status,
            Stonegrid_Bottom_Left_Bracket_Cam_3_Counter = plcData.Stonegrid_Bottom_Left_Bracket_Cam_3_Counter,
            Stonegrid_Top_Left_Slider_Cam_4_Status = plcData.Stonegrid_Top_Left_Slider_Cam_4_Status,
            Stonegrid_Top_Left_Slider_Cam_4_Counter = plcData.Stonegrid_Top_Left_Slider_Cam_4_Counter,
            Stonegrid_Bottom_Right_Bracket_Cam_5_Status = plcData.Stonegrid_Bottom_Right_Bracket_Cam_5_Status,
            Stonegrid_Bottom_Right_Bracket_Cam_5_Counter = plcData.Stonegrid_Bottom_Right_Bracket_Cam_5_Counter,
            Stonegrid_Top_Right_Slider_Cam_6_Status = plcData.Stonegrid_Top_Right_Slider_Cam_6_Status,
            Stonegrid_Top_Right_Slider_Cam_6_Counter = plcData.Stonegrid_Top_Right_Slider_Cam_6_Counter,
            Airduct_Left_Middle_Slider_Cam_4_Status = plcData.Airduct_Left_Middle_Slider_Cam_4_Status,
            Airduct_Left_Middle_Slider_Cam_4_Counter = plcData.Airduct_Left_Middle_Slider_Cam_4_Counter,
            Airduct_Right_Middle_Slider_Cam_6_Status = plcData.Airduct_Right_Middle_Slider_Cam_6_Status,
            Airduct_Right_Middle_Slider_Cam_6_Counter = plcData.Airduct_Right_Middle_Slider_Cam_6_Counter,
            Airduct_Left_Top_Bracket_Cam_7_Status = plcData.Airduct_Left_Top_Bracket_Cam_7_Status,
            Airduct_Left_Top_Bracket_Cam_7_Counter = plcData.Airduct_Left_Top_Bracket_Cam_7_Counter,
            Airduct_Right_Top_Bracket_Cam_8_Status = plcData.Airduct_Right_Top_Bracket_Cam_8_Status,
            Airduct_Right_Top_Bracket_Cam_8_Counter = plcData.Airduct_Right_Top_Bracket_Cam_8_Counter,
            Airduct_Left_Top_Balcony_Cam_9_Status = plcData.Airduct_Left_Top_Balcony_Cam_9_Status,
            Airduct_Left_Top_Balcony_Cam_9_Counter = plcData.Airduct_Left_Top_Balcony_Cam_9_Counter,
            Airduct_Right_Top_Balcony_Cam_10_Status = plcData.Airduct_Right_Top_Balcony_Cam_10_Status,
            Airduct_Right_Top_Balcony_Cam_10_Counter = plcData.Airduct_Right_Top_Balcony_Cam_10_Counter,
            Airduct_Middle_Bottom_Ridge_Cam_11_Status = plcData.Airduct_Middle_Bottom_Ridge_Cam_11_Status,
            Airduct_Middle_Bottom_Ridge_Cam_11_Counter = plcData.Airduct_Middle_Bottom_Ridge_Cam_11_Counter
        };
    }

    #endregion

    #region Stacja 2
    public static Station2ResultsData ToCoreModel(this LogixStation2ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station2ResultsData
        {
            SN = plcData.SN.ToString(),
            SN_Status = plcData.SN_Status,
            MFS_SN = plcData.MFS_SN.ToString(),
            MFS_Status = plcData.MFS_Status,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,

            MFS_Left_Bottom_Balcony_Cam_1_Status = plcData.MFS_Left_Bottom_Balcony_Cam_1_Status,
            MFS_Left_Bottom_Balcony_Cam_1_Counter = plcData.MFS_Left_Bottom_Balcony_Cam_1_Counter,
            MFS_Left_Middle_Guide_Cam_2_Status = plcData.MFS_Left_Middle_Guide_Cam_2_Status,
            MFS_Left_Middle_Guide_Cam_2_Counter = plcData.MFS_Left_Middle_Guide_Cam_2_Counter,
            MFS_Left_Top_Bracket_Cam_3_Status = plcData.MFS_Left_Top_Bracket_Cam_3_Status,
            MFS_Left_Top_Bracket_Cam_3_Counter = plcData.MFS_Left_Top_Bracket_Cam_3_Counter,
            MFS_Middle_Top_Ridge_Cam_4_Status = plcData.MFS_Middle_Top_Ridge_Cam_4_Status,
            MFS_Middle_Top_Ridge_Cam_4_Counter = plcData.MFS_Middle_Top_Ridge_Cam_4_Counter,
            MFS_Right_Top_Bracket_Cam_5_Status = plcData.MFS_Right_Top_Bracket_Cam_5_Status,
            MFS_Right_Top_Bracket_Cam_5_Counter = plcData.MFS_Right_Top_Bracket_Cam_5_Counter,
            MFS_Right_Middle_Guide_Cam_6_Status = plcData.MFS_Right_Middle_Guide_Cam_6_Status,
            MFS_Right_Middle_Guide_Cam_6_Counter = plcData.MFS_Right_Middle_Guide_Cam_6_Counter,
            MFS_Right_Bottom_Balcony_Cam_7_Status = plcData.MFS_Right_Bottom_Balcony_Cam_7_Status,
            MFS_Right_Bottom_Balcony_Cam_7_Counter = plcData.MFS_Right_Bottom_Balcony_Cam_7_Counter,
            Radiator_Left_Top_Grommet_Cam_8_Status = plcData.Radiator_Left_Top_Grommet_Cam_8_Status,
            Radiator_Left_Top_Grommet_Cam_8_Counter = plcData.Radiator_Left_Top_Grommet_Cam_8_Counter,
            Radiator_Right_Top_Grommet_Cam_9_Status = plcData.Radiator_Right_Top_Grommet_Cam_9_Status,
            Radiator_Right_Top_Grommet_Cam_9_Counter = plcData.Radiator_Right_Top_Grommet_Cam_9_Counter,
            Shipping_Caps_Disassembly_Status = plcData.Shipping_Caps_Disassembly_Status,
            Shipping_Caps_Disassembly_Counter = plcData.Shipping_Caps_Disassembly_Counter
        };
    }

    #endregion

    #region Stacja 3
    public static Station3ResultsData ToCoreModel(this LogixStation3ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station3ResultsData
        {
            SN = plcData.DMC.ToString(),
            SN_Status = plcData.ResultCode,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,
            LeakTest_Status = plcData.LeakTest_Status,
            LeakTest_Counter = plcData.LeakTest_Counter,
            LeakTest_Method = plcData.LeakTest_Method,
            LeakTest_Program_Number = plcData.LeakTest_Program_Number,
            LeakTest_Pressure = plcData.LeakTest_Pressure,
            LeakTest_Pressure_Drop = plcData.LeakTest_Pressure_Drop,
            LeakTest_Leak_Rate = plcData.LeakTest_Leak_Rate,
            LeakTest_Temperature = plcData.LeakTest_Temperature
        };
    }

    #endregion

    #region Stacja 4
    public static Station4ResultsData ToCoreModel(this LogixStation4ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station4ResultsData
        {
            SN = plcData.DMC.ToString(),
            SN_Status = plcData.ResultCode,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,
            Shipping_Caps_Assembly_Status = plcData.Shipping_Caps_Assembly_Status,
            Shipping_Caps_Assembly_Counter = plcData.Shipping_Caps_Assembly_Counter,
            Print_Counter = plcData.Print_Counter,
            DMC_Quality_Content_Status = plcData.DMC_Quality_Content_Status,
            DMC_Quality_Content_Counter = plcData.DMC_Quality_Content_Counter,
            DMC_Quality_Grade_Status = plcData.DMC_Quality_Grade_Status,
            DMC_Quality_Grade_Counter = plcData.DMC_Quality_Grade_Counter,
            DMC_Quality_Grade_Result = ((char)plcData.DMC_Quality_Grade_Result).ToString(),
            DMC_OCR_Homologation_Part_Number_Status = plcData.DMC_OCR_Homologation_Part_Number_Status,
            DMC_OCR_Homologation_Part_Number_Counter = plcData.DMC_OCR_Homologation_Part_Number_Counter,
            DMC_OCR_Supplier_Status = plcData.DMC_OCR_Supplier_Status,
            DMC_OCR_Supplier_Counter = plcData.DMC_OCR_Supplier_Counter,
            DMC_OCR_Country_Of_Origin_Status = plcData.DMC_OCR_Country_Of_Origin_Status,
            DMC_OCR_Country_Of_Origin_Counter = plcData.DMC_OCR_Country_Of_Origin_Counter
        };
    }
    
    #endregion

    #region Stacja 5 Mfs1
    public static Station5ResultsData ToCoreModel(this LogixStation5ResultsData plcData)
    {
        if (plcData == null) return null;

        int ScrewStatus1 = 0; 
        int ScrewStatus2 = 0;
        int ScrewStatus3 = 0;

        if (plcData.Screw[0].Status_OK)
            ScrewStatus1 = 1;
        else if (plcData.Screw[0].Status_NOK)
            ScrewStatus1 = 2;
        else
            ScrewStatus1 = 0;

        if (plcData.Screw[1].Status_OK)
            ScrewStatus2 = 1;
        else if (plcData.Screw[1].Status_NOK)
            ScrewStatus2 = 2;
        else
            ScrewStatus2 = 0;

        if (plcData.Screw[2].Status_OK)
            ScrewStatus3 = 1;
        else if (plcData.Screw[2].Status_NOK)
            ScrewStatus3 = 2;
        else
            ScrewStatus3 = 0;

        return new Station5ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,

            Motor_Inspection_Status = plcData.Motor_Inspection_Status,
            Motor_Inspection_Counter = plcData.Motor_Inspection_Counter,
            Fan_Inspection_Status = plcData.Fan_Inspection_Status,
            Fan_Inspection_Counter = plcData.Fan_Inspection_Counter,

            Screw1_Status = ScrewStatus1,
            Screw1_Counter = plcData.Screw[0].Counter,
            Screw1_Program_Number = plcData.Screw[0].Program_Number,
            Screw1_Angle = plcData.Screw[0].Angle,
            Screw1_Torque = plcData.Screw[0].Torque,
            Screw2_Status = ScrewStatus2,
            Screw2_Counter = plcData.Screw[1].Counter,
            Screw2_Program_Number = plcData.Screw[1].Program_Number,
            Screw2_Angle = plcData.Screw[1].Angle,
            Screw2_Torque = plcData.Screw[1].Torque,
            Screw3_Status = ScrewStatus3,
            Screw3_Counter = plcData.Screw[2].Counter,
            Screw3_Program_Number = plcData.Screw[2].Program_Number,
            Screw3_Angle = plcData.Screw[2].Angle,
            Screw3_Torque = plcData.Screw[2].Torque,
        };
    }

    #endregion

    #region Stacja 6 Balancing
    public static Station6ResultsData ToCoreModel(this LogixStation6ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station6ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,

            Initial_static_unbalance = Math.Round(plcData.Initial_static_unbalance, 3),
            Final_static_unbalance = Math.Round(plcData.Final_static_unbalance, 3),
            Runout = Math.Round(plcData.Runout, 3),
            Initial_moment_unbalance = Math.Round(plcData.Initial_moment_unbalance, 3),
            Final_moment_unbalance = Math.Round(plcData.Final_moment_unbalance, 3),
            Chamber = Math.Round(plcData.Chamber, 3),
            Initial_static_unbalance_angle = Math.Round(plcData.Initial_static_unbalance_angle, 3),
            Final_static_unbalance_angle = Math.Round(plcData.Final_static_unbalance_angle, 3),
            Rpm = Math.Round(plcData.Rpm, 3),
            Current = Math.Round(plcData.Current, 3)
        };
    }

    #endregion

    #region Stacja 7 Mfs3

    public static LogixStationWriteBase ToPlcStructure(this Station7WriteBaseModel coreModel)
    {
        if (coreModel == null) return null;

        return new LogixStationWriteBase
        {
            Life = (short)coreModel.Life,
            Task_Counter = (ushort)coreModel.TaskCounter,
            // Tutaj mapujemy parametry sterujące wysyłane z PC:
            Task_Recived_From_PC = (short)coreModel.TaskSendToPLC,
            Task_Confirm_From_PC = (short)coreModel.TaskConfirmToPLC,
            Task_Succes_Counter = (ushort)coreModel.TaskSuccessCounter,
            Task_Status = (short)coreModel.TaskStatus,
            Camera_Found = coreModel.CameraFound,
            Camera_Moved = coreModel.CameraMoved,
            AdditionalInt1 = (short)coreModel.BalancingStatus


            // Rezerwy pozostają domyślnie 0
        };
    }

    public static Station7ResultsData ToCoreModel(this LogixStation7ResultsData plcData)
    {
        if (plcData == null) return null;

        int ScrewStatus1 = 0;
        int ScrewStatus2 = 0;
        int ScrewStatus3 = 0;

        if (plcData.Screw[0].Status_OK)
            ScrewStatus1 = 1;
        else if (plcData.Screw[0].Status_NOK)
            ScrewStatus1 = 2;
        else
            ScrewStatus1 = 0;

        if (plcData.Screw[1].Status_OK)
            ScrewStatus2 = 1;
        else if (plcData.Screw[1].Status_NOK)
            ScrewStatus2 = 2;
        else
            ScrewStatus2 = 0;

        if (plcData.Screw[2].Status_OK)
            ScrewStatus3 = 1;
        else if (plcData.Screw[2].Status_NOK)
            ScrewStatus3 = 2;
        else
            ScrewStatus3 = 0;

        return new Station7ResultsData
        {
            Motor_SN = plcData.Motor_Sn.ToString(),
            Motor_Status = plcData.Motor_Status,
            Mfs_SN = plcData.Mfs_Sn.ToString(),
            Mfs_Status = plcData.Mfs_Status,
            CycleTime = plcData.CycleTime,
            Reference_Number = plcData.Reference_Number,

            Shraud_Status = plcData.Shraud_Status,
            Shraud_Counter = plcData.Shraud_Counter,
            Cable_Status = plcData.Cable_Status,
            Cable_Counter = plcData.Cable_Counter,
            Plug_Status = plcData.Plug_Status,
            Plug_Counter = plcData.Plug_Counter,
            Shipping_Cap_Status = plcData.Shipping_Cap_Status,
            Shipping_Cap_Counter = plcData.Shipping_Cap_Counter,

            Print_Counter = plcData.Print_Counter,
            DMC_Quality_Content_Status = plcData.DMC_Quality_Content_Status,
            DMC_Quality_Content_Counter = plcData.DMC_Quality_Content_Counter,
            DMC_Quality_Grade_Status = plcData.DMC_Quality_Grade_Status,
            DMC_Quality_Grade_Counter = plcData.DMC_Quality_Grade_Counter,
            DMC_Quality_Grade_Result = ((char)plcData.DMC_Quality_Grade_Result).ToString(),
            DMC_OCR_Homologation_Part_Number_Status = plcData.DMC_OCR_Homologation_Part_Number_Status,
            DMC_OCR_Homologation_Part_Number_Counter = plcData.DMC_OCR_Homologation_Part_Number_Counter,
            DMC_OCR_Supplier_Status = plcData.DMC_OCR_Supplier_Status,
            DMC_OCR_Supplier_Counter = plcData.DMC_OCR_Supplier_Counter,
            DMC_OCR_Country_Of_Origin_Status = plcData.DMC_OCR_Country_Of_Origin_Status,
            DMC_OCR_Country_Of_Origin_Counter = plcData.DMC_OCR_Country_Of_Origin_Counter,

            Screw1_Status = ScrewStatus1,
            Screw1_Counter = plcData.Screw[0].Counter,
            Screw1_Program_Number = plcData.Screw[0].Program_Number,
            Screw1_Angle = plcData.Screw[0].Angle,
            Screw1_Torque = plcData.Screw[0].Torque,
            Screw2_Status = ScrewStatus2,
            Screw2_Counter = plcData.Screw[1].Counter,
            Screw2_Program_Number = plcData.Screw[1].Program_Number,
            Screw2_Angle = plcData.Screw[1].Angle,
            Screw2_Torque = plcData.Screw[1].Torque,
            Screw3_Status = ScrewStatus3,
            Screw3_Counter = plcData.Screw[2].Counter,
            Screw3_Program_Number = plcData.Screw[2].Program_Number,
            Screw3_Angle = plcData.Screw[2].Angle,
            Screw3_Torque = plcData.Screw[2].Torque,
        };
    }

    #endregion

    #region Stacja 8 Rework

    public static Station8ReadBaseModel ToCoreModel8(this LogixStationReadBase plcData)
    {
        if (plcData == null) return null;

        return new Station8ReadBaseModel
        {
            Life = plcData.Life,
            TaskCounter = plcData.Task_Counter,
            TaskSendToPc = plcData.Task_Send_To_PC,
            TaskConfirmToPc = plcData.Task_Confirm_To_PC,
            TaskSuccessCounter = plcData.Task_Succes_Counter,
            TaskStatus = plcData.Task_Status,
            DMC = plcData.DMC.ToString(),
            DMCCamera = plcData.DMC_Camera.ToString(),
            CameraRequest = plcData.CameraRequest,
            Rework_Code = plcData.Reserve1
        };
    }

    public static Station8ResultsData ToCoreModel(this LogixStation8ResultsData plcData)
    {
        if (plcData == null) return null;

        return new Station8ResultsData
        {
            Reserve1_SN = plcData.Reserve1_Sn.ToString(),
            Reserve1_Status = plcData.Reserve1_Status,
            Reserve2_SN = plcData.Reserve2_Sn.ToString(),
            Reserve2_Status = plcData.Reserve2_Status,
        };
    }

    #endregion
}
