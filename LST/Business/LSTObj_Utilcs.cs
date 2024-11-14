//$Header:$
//
//  Reports.cs
//  Developed by Madhu Swaminathan
//  Copyright (c) 2016 SLAC. All rights reserved.
//
//  This is the utility class with all the objects and their properties of LST
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LST.Business
{
    public class LSTObj_Utilcs
    {
    }

    public class Facility
    {
        public int FacilityId { get; set; }
        public string FacilityName { get; set; }
        public string Bldg { get; set; }
        public string Room { get; set; }
        public string OtherLocation { get; set; }
        public int SLSO { get; set; }
        public int ActSLSO { get; set; }
        public int CoSLSO1 { get; set; }
        public int CoSLSO2 { get; set; }
        public int AltSLSO { get; set; }
        public DateTime AltSLSOFrom { get; set; }
        public DateTime AltSLSOTo { get; set; }
        public string FacWebpage { get; set; }
        public DateTime SopRevisedDate { get; set; }
        public DateTime ApprovalExpDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string SLSOName { get; set; }
        public string ActSLSOName { get; set; }
        public string CoSLSOName { get; set; }
        public string AltSLSOName { get; set; }
        public string LinkText { get; set; }
        public string LinkUrl { get; set; }
        public int PMId { get; set; }
        public int CoordId { get; set; }
        public string PMName { get; set; }
        public string CoordName { get; set; }
        public string SLSOSlacId { get; set; }
        
    }

    public class Worker
    {
        public int WorkerId { get; set; }
        public int SlacId { get; set; }
        public int AffiliationId { get; set; }
        public string PreferredEmail { get; set; }
        public DateTime ManualReviewDate { get; set; }
        public DateTime StudentReqReviewDate { get; set; }
        public string IsActive {get;set;}
        public string CreatedById { get; set; }
        public string CreatedOn { get; set; }
        public string AlternateSvr { get; set; }
        public DateTime AlternateSvrFrom { get; set; }
        public DateTime AlternateSvrTo { get; set; }
        public string WorkerName { get; set; }
        public string Affiliation { get; set; }
        public string Creator { get; set; }
        public string AltSvrName { get; set; }
        public string BadgeId { get; set; }
        public string EmailAddr { get; set; }
        public string IsESHManualReviewed { get; set; }
        public string IsStudReqReviewed { get; set; }
        public string ModifiedBy { get; set; }
        public int StatusId { get; set; }
        public string Status { get; set; }
        public string Supervisor { get; set; }
      
    }

    public class User
    {
        public int UserRoleId { get; set; }
        public int RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public int SlacId { get; set; }
        public string CreatedBy { get; set; }
        public string ModifiedBy { get; set; }
        public string UserName { get; set; }
        public DateTime AlternateLSOFrom { get; set; }
        public DateTime AlternateLSOTo { get; set; }
    }

    public class WorkerFacility
    {
        public int MapId { get; set; }
        public int WorkerId { get; set; }
        public int FacilityId { get; set; }
        public int WorkTypeId { get; set; }
        public string ConditionalApproval { get; set; }
        public string Justification { get; set; }
        public string SOPReviewed { get; set; }
        public DateTime SOPReviewDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime OJTCompletionDate { get; set; }
        public int StatusId { get; set; }
        public string FacilityName { get; set; }
        public int SlacId { get; set; }
        public string Worker { get; set; }
        public string WorkType { get; set; }
        public string Status { get; set; }
    }

    public class Approval
    {
        public int ApprovalId { get; set; }
        public string Comments { get; set; }
        public string Status { get; set; }
        public int StatusId { get; set; }
        public int ApproverId { get; set; }
        public DateTime ApprovedOn { get; set; }
        public int MapId { get; set; }
        public string ApproverType { get; set; }
        public string Approver { get; set; }
        
    }

    public class FacilityRequest
    {
        public int RequestId { get; set; }
        public int FacilityId { get; set; }
        public int RequestStatusId { get; set; }
        public DateTime NewFacilityDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string RequestStatus { get; set; }
        public string IsSOPCompleted { get; set; }
        public string IsAnnualCertCompleted { get; set; }
        public string IsSOPRevApproved { get; set; }
        public string ModifiedBy { get; set; }
    }

    public class FacilityApproval
    {
        public int ApprovalId { get; set; }
        public int RequestId { get; set; }
        public string ApproverType { get; set; }
        public int ApproverId { get; set; }
        public int StatusId { get; set; }
        public DateTime ActionDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Comments { get; set; }
        public string ApproverName { get; set; }
        public string ApprovalStatus { get; set; }
      

      
    }

    public class OJTFields
    {
        public int FieldId { get; set; }
        public string Columnlabel { get; set; }
        public string CreatedBy { get; set; }
    }
    
    public class OJTFieldMatrix
    {
        public int FieldMatrixId { get; set; }
        public int WorkerFacMapId { get; set; }
        public string WorkerName { get; set; }
        public int FacilityId { get; set; }
        public string FieldValue { get; set; }
        public int OJTFieldId { get; set; }
    }


    public class CustomEmail
    {
        public int FacId { get; set; }
        public int ListId { get; set; }
        public string Subject { get; set; }
        public string BodyMsg { get; set; }
        public string CreatedBy { get; set; }
    }

}