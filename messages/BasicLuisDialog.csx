using System;
using System.Data;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Net;
using System.Web;
using System.Web.Http;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;

//using Microsoft.Bot.Connector.Utilities;

// For more information about this template visit http://aka.ms/azurebots-csharp-luis
[Serializable]
 /*//  public class EchoDialog : IDialog<object>
{
    public async Task StartAsync(IDialogContext context)
    {
        context.Wait(MessageReceivedAsync);
    }

    public async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
    {
        var message = await argument;
        await context.PostAsync("You said: " + message.Text);
        context.Wait(MessageReceivedAsync);
    }
}
 public virtual async Task<HttpResponseMessage> Post([FromBody] Activity activity)
{
    // Check if activity is of type message
    if (activity != null && activity.GetActivityType() == ActivityTypes.Message)
    {
        await Conversation.SendAsync(activity, () => new EchoDialog());
    }
    else
    {
        HandleSystemMessage(activity);
    }
    return new HttpResponseMessage(System.Net.HttpStatusCode.Accepted);
} */  
public class BasicLuisDialog : LuisDialog<object>
{
    public BasicLuisDialog() : base(new LuisService(new LuisModelAttribute(Utils.GetAppSetting("LuisAppId"), Utils.GetAppSetting("LuisAPIKey"))))
    {
    }
   
    // Go to https://luis.ai and create a new intent, then train/publish your luis app.
    // Finally replace "MyIntent" with the name of your newly created intent in the following handler
    //public Microsoft.Bot.Connector.ChannelAccount From { get; set; }
    //public string ContentType;
   // public Activity (ContentType);

    [LuisIntent("None")]
    public async Task NoneIntent(IDialogContext context, LuisResult result)
    {
      
        await context.PostAsync($"You have reached the none intent. You said: {result.Query}"); //
        
        context.Wait(MessageReceived);
    }
    [LuisIntent("Status")]
    public async Task StatusIntent(IDialogContext context, LuisResult result)
    {
       string resultvalue = result.Query;
       // string[] separators = {",", ".", "!", "?", ";", ":", " "};
     // string resultvalue = "1088988 mystatus";
       // string[] words = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
       string ticketid = resultvalue.Replace(" status" , "").Trim();
        var statusentities = result.Entities;
        EntityRecommendation statusEntity = new EntityRecommendation();
        if (result.TryFindEntity("requestID", out statusEntity))
        await context.PostAsync($" You have requested to get status for : {statusEntity.Entity}. Please give me a moment while i fetch it");
        {//Making Web Request    
        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(@"http://52.172.24.13:8181/ws/ChatBotProcess");    
        //SOAPAction    
        request1.Headers.Add(@"SOAPAction:");    
        //Content_type    
        request1.ContentType = "text/xml;charset=\"utf-8\"";    
        request1.Accept = "text/xml";    
        //HTTP method    
        request1.Method = "POST";   
        NetworkCredential cred1 = new NetworkCredential("admin", "red9door");
        request1.Credentials = cred1;
        XmlDocument SOAPReqBody1 = new XmlDocument();  
        //SOAP Body Request   
        //SOAPReqBody1.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:launchnotepad""><soapenv:Header/><soapenv:Body><urn:Action1 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><bpInstance xsi:type=""xsd:string"">"+@"01603b7f-0f73-4546-81d7-f7215f2b386b"+@"</bpInstance><Path xsi:type=""xsd:string"">C:\Program Files\Internet Explorer\iexplore.exe</Path></urn:Action1></soapenv:Body></soapenv:Envelope>");
        SOAPReqBody1.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:chatbotprocess""><soapenv:Header/><soapenv:Body><urn:ChatBotProcess soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><ProcessName xsi:type=""xsd:string"">Status</ProcessName><ticketnumber xsi:type=""xsd:string"">"+statusEntity.Entity+@"</ticketnumber></urn:ChatBotProcess></soapenv:Body></soapenv:Envelope>");
        using(Stream stream1 = request1.GetRequestStream()) 
            {  
            SOAPReqBody1.Save(stream1);  
            } 
         //Geting response from request    
       using(WebResponse Serviceres = request1.GetResponse()) 
        {  
        using(StreamReader rd = new StreamReader(Serviceres.GetResponseStream())) 
            {  
            //reading stream    
            var ServiceResult = rd.ReadToEnd();
            //writting stream result on console    
            await context.PostAsync(ServiceResult); 
            }  
        } 
        
            }
        /*SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
         builder.DataSource = "eyunidbserver.database.windows.net"; 
         builder.UserID = "eyunidbserver";            
         builder.Password = "Password@123";     
         builder.InitialCatalog = "EYUNIDB";
        using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
         {
            //  connection.Close();
                  
              connection.Open(); 
            //SqlDataReader myreader = null;
              //string sql = string.Concat("Select STATUS from sampletable where TICKETNUMBER = ",ticketid,")");
              string sql = $"Select STATUS from sampletable where TICKETNUMBER = {ticketid}";
              SqlCommand command = new SqlCommand(sql, connection);
              //myReader = command.ExecuteReader();
            SqlDataReader reader;
             string re = "";
            reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                             re = reader.GetValue(i).ToString();
                        }
                        
            }
             
             if (re.Trim() == "Completed")
              {
                await context.PostAsync(string.Concat("Your request with ID:",ticketid," is completed. Thanks & Visit again!"));
                connection.Close();
                context.Wait(MessageReceived);
              }
              
               if (re.Trim() == "Submit")
              {
                await context.PostAsync(string.Concat("Your request with ID:",ticketid," is not completed. Please try after sometime."));
                connection.Close();
                context.Wait(MessageReceived);
              }
              
              if (re.Trim() == "")
              {
                await context.PostAsync(string.Concat("The ticket number ",ticketid," is invalid. Please try with a valid ticket number."));
                //await context.PostAsync(re);
                context.Wait(MessageReceived);
                connection.Close();
              }    
          //await context.PostAsync (re);
         }
        */  
         
    }
    [LuisIntent("Greeting")]
    public async Task GreetingIntent(IDialogContext context, LuisResult result)
           {  

            var form = new ExpenseForm();

            var entities = new List<EntityRecommendation>(result.Entities);

            var formDialog = new FormDialog<ExpenseForm>(form, BuildForm, FormOptions.PromptInStart, entities);
            
            context.Call(formDialog, OnComplete);
      await context.PostAsync("Hi there, How can I help?"); 
      
           /*//await context.PostAsync($"Run Onboarding process (Please enter '1') {Environment.NewLine} Run EmailGuide process (Please enter '2')"); //
      context.Wait(MessageReceived);*/
          }
              [LuisIntent("Process")]
    public async Task ProcessIntent(IDialogContext context, LuisResult result)
    {
      // Finally replace "MyIntent" with the name of your newly created intent in the following handler
      // await context.PostAsync($"You have reached the Greeting intent. You said: {result.Query}");
      context.Wait(MessageReceived);
    //var token = "";
    var timeDiff=DateTime.UtcNow - new DateTime(2017, 5, 5);
    int ID = Convert.ToInt32(timeDiff.TotalSeconds);
    var entities = result.Entities;
    EntityRecommendation processEntity = new EntityRecommendation();
    if (result.TryFindEntity("ProcessName", out processEntity))
        //var processName = processEntity.Entity;
        await context.PostAsync($"You have requested to run the process : {processEntity.Entity}.");
        await context.PostAsync($"Please give me a moment while I submit it.");
        //await context.PostAsync($"{ID}.");
            {//Making Web Request    
        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(@"http://52.172.24.13:8181/ws/ChatBotProcess");    
        //SOAPAction    
        request1.Headers.Add(@"SOAPAction:");    
        //Content_type    
        request1.ContentType = "text/xml;charset=\"utf-8\"";    
        request1.Accept = "text/xml";    
        //HTTP method    
        request1.Method = "POST";   
        NetworkCredential cred1 = new NetworkCredential("admin", "red9door");
        request1.Credentials = cred1;
        XmlDocument SOAPReqBody1 = new XmlDocument();  
        //SOAP Body Request   
        //SOAPReqBody1.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:launchnotepad""><soapenv:Header/><soapenv:Body><urn:Action1 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><bpInstance xsi:type=""xsd:string"">"+@"01603b7f-0f73-4546-81d7-f7215f2b386b"+@"</bpInstance><Path xsi:type=""xsd:string"">C:\Program Files\Internet Explorer\iexplore.exe</Path></urn:Action1></soapenv:Body></soapenv:Envelope>");
        SOAPReqBody1.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:chatbotprocess""><soapenv:Header/><soapenv:Body><urn:ChatBotProcess soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><ProcessName xsi:type=""xsd:string"">"+processEntity.Entity+@"</ProcessName><ticketnumber xsi:type=""xsd:string"">"+ID+@"</ticketnumber></urn:ChatBotProcess></soapenv:Body></soapenv:Envelope>");
        using(Stream stream1 = request1.GetRequestStream()) 
            {  
            SOAPReqBody1.Save(stream1);  
            } 
         //Geting response from request    
       using(WebResponse Serviceres = request1.GetResponse()) 
        {  
        using(StreamReader rd = new StreamReader(Serviceres.GetResponseStream())) 
            {  
            //reading stream    
            var ServiceResult = rd.ReadToEnd();
            //writting stream result on console    
            await context.PostAsync(ServiceResult); 
            }  
        } 
        
            }
    }
    [LuisIntent("Thanks")]
    public async Task ThanksIntent(IDialogContext context, LuisResult result)
    {
        
      // Finally replace "MyIntent" with the name of your newly created intent in the following handler
      // await context.PostAsync($"You have reached the Greeting intent. You said: {result.Query}");
      await context.PostAsync("Sure! Let me know if you need something else."); 
      //await context.PostAsync($"Run Onboarding process (Please enter '1') {Environment.NewLine} Run EmailGuide process (Please enter '2')"); //
      context.Wait(MessageReceived);
    
      }
    /*[LuisIntent("Options")]
    public async Task OptionsIntent(IDialogContext context, LuisResult result)
    {
         // Finally replace "MyIntent" with the name of your newly created intent in the following handler

         SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
         builder.DataSource = "eyunidbserver.database.windows.net"; 
         builder.UserID = "eyunidbserver";            
         builder.Password = "Password@123";     
         builder.InitialCatalog = "EYUNIDB";

       if (result.Query == "1")
        {
            await context.PostAsync("You have requested to run Onboarding process"); 
            
         using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
         {

              connection.Open(); 
              var timeDiff=DateTime.UtcNow - new DateTime(2017, 5, 5);
              int ID = Convert.ToInt32(timeDiff.TotalSeconds);
              string sql = string.Concat("INSERT INTO [sampletable] values (1 , 'Onboarding', 'Submit',",ID,")");
              SqlCommand command = new SqlCommand(sql, connection);
              int rowsAffected = command.ExecuteNonQuery();    
              string message = string.Concat("Submitted successfully. Your Request ID: ",ID);
              await context.PostAsync(message);
             await context.PostAsync("If you want your ticket status. Please send query in this format [RequestID status] (Eg: 1233356 status)");
             //await context.PostAsync(output);
              context.Wait(MessageReceived);
              connection.Close();
         }
            
        }
        
      
         if (result.Query == "2")
        {
            var token = "";
            {
        await context.PostAsync("You have requested to run Launch Weather Monitor process."); 
       //Making Web Request    
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"http://13.65.98.49:8181/ws/LaunchNotepad");    
        //SOAPAction    
        request.Headers.Add(@"SOAPAction:");    
        //Content_type    
        request.ContentType = "text/xml;charset=\"utf-8\"";    
        
        request.Accept = "text/xml";    
        //HTTP method    
        request.Method = "POST";   
        NetworkCredential cred = new NetworkCredential("admin", "eybp123");
        request.Credentials = cred;
        XmlDocument SOAPReqBody = new XmlDocument();  
        //SOAP Body Request    
        SOAPReqBody.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:launchnotepad""> <soapenv:Header/> <soapenv:Body> <urn:Initialise soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""/> </soapenv:Body> </soapenv:Envelope>");   
         using(Stream stream = request.GetRequestStream()) {  
        SOAPReqBody.Save(stream);  
        } 
         //Geting response from request    
       using(WebResponse Serviceres = request.GetResponse()) {  
       using(StreamReader rd = new StreamReader(Serviceres.GetResponseStream())) {  
           //reading stream    
           token = rd.ReadToEnd();  
           //writting stream result on console    
          await context.PostAsync(token); 
         }  
      } 
        }
      {       //Making Web Request    
        HttpWebRequest request1 = (HttpWebRequest)WebRequest.Create(@"http://13.65.98.49:8181/ws/LaunchNotepad");    
        //SOAPAction    
        request1.Headers.Add(@"SOAPAction:");    
        //Content_type    
        request1.ContentType = "text/xml;charset=\"utf-8\"";    
        
        request1.Accept = "text/xml";    
        //HTTP method    
        request1.Method = "POST";   
        NetworkCredential cred1 = new NetworkCredential("admin", "eybp123");
        request1.Credentials = cred1;
        XmlDocument SOAPReqBody1 = new XmlDocument();  
        //SOAP Body Request   
        await context.PostAsync(token);
        SOAPReqBody1.LoadXml(@"<soapenv:Envelope xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:soapenv=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:urn=""urn:blueprism:webservice:launchnotepad""><soapenv:Header/><soapenv:Body><urn:Action1 soapenv:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/""><bpInstance xsi:type=""xsd:string"">"+@"01603b7f-0f73-4546-81d7-f7215f2b386b"+@"</bpInstance><Path xsi:type=""xsd:string"">C:\Program Files\Internet Explorer\iexplore.exe</Path></urn:Action1></soapenv:Body></soapenv:Envelope>");
        using(Stream stream1 = request1.GetRequestStream()) 
            {  
            SOAPReqBody1.Save(stream1);  
            } 
         //Geting response from request    
       using(WebResponse Serviceres = request1.GetResponse()) 
        {  
        using(StreamReader rd = new StreamReader(Serviceres.GetResponseStream())) 
            {  
            //reading stream    
            var ServiceResult = rd.ReadToEnd();
            //writting stream result on console    
            await context.PostAsync(ServiceResult); 
            }  
        } 
          
      }
        }

    }*/
}
public enum RequestType { Expense = 1, Timesheet };
public enum ExpenseType { Meal = 1, Conveyance };
public enum IndividualType { Individual = 1, Group };

public class ExpenseForm
{
        [Prompt("Hey there! I am your GTnE Genie! I can file your Expense or fill your Timesheet! What is it that you would want me to do today? {||}")]
    public RequestType Request { get; set; }
    
    [Prompt("I would need the date on which this expense was incurred?")]
    public string DateInc { get; set; }
    
    [Prompt("Please help with me with the type of expense by selecting one of the below: {||}")]
    public ExpenseType Expense { get; set; }
    
    [Prompt("Please share a basic description for this expense")]
    public string Description { get; set; }
    
    [Prompt("Is this expense individual or group? {||}")]
    public IndividualType IndType { get; set; }
    
    [Prompt("Please share the Engagement code we need to charge this on? ")]
    public string EngCode { get; set; }
    
    [Prompt("Please share the location where this expense occur?")]
    public string ExpLocation { get; set; }
    
    [Prompt("Please provide the Amount for this meal?")]
    public string ExpAmt { get; set; }

    public static IForm<BasicForm> BuildForm()
    {
        // Builds an IForm<T> based on BasicForm
        return new FormBuilder<BasicForm>().Build();
    }

    public static IFormDialog<BasicForm> BuildFormDialog(FormOptions options = FormOptions.PromptInStart)
    {
        // Generated a new FormDialog<T> based on IForm<BasicForm>
        return FormDialog.FromForm(BuildForm, options);
    }
}