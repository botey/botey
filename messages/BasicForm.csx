using System;
using Microsoft.Bot.Builder.FormFlow;

public enum RequestType { Expense = 1, Timesheet };
public enum ExpenseType { Meal = 1, Conveyance };
public enum TeamType { Individual = 1, Group };

// For more information about this template visit http://aka.ms/azurebots-csharp-form
[Serializable]
public class BasicForm
{

    [Prompt("Hey there! I am your GTnE Genie! I can file your Expense or fill your Timesheet! What is it that you would want me to do today? {||}")]
    public RequestType RequestType { get; set; }

    [Prompt("I would need the date on which this expense was incurred?")]
    public string DateIncurred { get; set; }

    [Prompt("Please help with me with the type of expense by selecting one of the below: {||}")]
    public ExpenseType ExpenseType { get; set; }

    [Prompt("Please share a basic description for this expense")]
    public string Description { get; set; }

    [Prompt("Is this expense individual or group? {||}")]
    public TeamType TeamType { get; set; }

    [Prompt("Please share the Engagement code we need to charge this on? ")]
    public string EngagementCode { get; set; }

    [Prompt("Please share the location where this expense occur?")]
    public string ExpenseLocation { get; set; }

    [Prompt("Please provide the Amount for this meal?")]
    public string ExpenseAmt { get; set; }

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

