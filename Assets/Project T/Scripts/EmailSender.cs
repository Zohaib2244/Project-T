using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using UnityEngine;

public class EmailSender : MonoBehaviour
{
    public string smtpServer = "smtp.gmail.com"; // SMTP server for Gmail
    public int smtpPort = 587; // Port number for Gmail SMTP
    public string smtpUser = "zohaibangry123@gmail.com"; // Your Gmail address
    public string smtpPassword = "dffokqvwflpeujrq"; // Your Gmail password
    public string fromEmail = "zohaibangry1234@gmail.com"; // Your Gmail address (same as smtpUser)
    public string subject = "Welcome To NDO 2024, Please see the necessary links for our Tabs";
    public string body = "this is an autoemail agoogoo for a gaga";
    public string link = "smthhelloworld.com";
public void SendEmail()
{
    try
    {
        Debug.Log("Starting to configure SMTP client...");

        SmtpClient client = new SmtpClient(smtpServer, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPassword),
            EnableSsl = true
        };

        Debug.Log("SMTP client configured successfully.");

        MailMessage mailMessage = new MailMessage
        {
            From = new MailAddress(fromEmail),
            Subject = subject,
            Body = body
        };
        mailMessage.To.Add("zohaibangry420@gmail.com");

        Debug.Log("Mail message created successfully.");

        client.Send(mailMessage);

        Debug.Log("Email sent successfully.");
    }
    catch (System.Exception ex)
    {
        Debug.LogError($"Failed to send email: {ex.Message}");
    }
}

    public void SendEmailsToAllSpeakers()
    {
        foreach (var team in AppConstants.instance.selectedTouranment.teamsInTourney)
        {
            foreach (var speaker in team.speakers)
            {
                subject = "Welcome To NDO 2024, Please see the necessary links for our Tabs";
                body = $"Hello {speaker.speakerName},\n\n" +
                              $"Your team for {AppConstants.instance.selectedTouranment.tournamentName} has been successfully registered. We are pleased to host you and your team.\n\n" +
                              $"Kindly verify your information:\n" +
                              $"Name: {speaker.speakerName}\n" +
                              $"Institute: {AppConstants.instance.GetInstituitionsFromID(team.instituition).instituitionName}\n" +
                              $"Contact: {speaker.speakerContact}\n\n" +
                              $"Team: {team.teamName}\n" +
                              $"Team Category: {team.teamCategory}\n\n" +
                              $"For any corrections, Kindly Contact: Ali Abdullah, +92 319 2619774\n" +
                              $"Please see the following link for our tabs site: {link}\n\n" +
                              $"Thank You.\n\n" +
                              $"This is an auto generated email, sent from tabX a tab app made by some very nutty people.";
                SendEmail(speaker.speakerEmail, subject, body);
            }
        }
    }
    public void SendEmailToAdjudicators()
    {
        foreach (var adjudicator in AppConstants.instance.selectedTouranment.adjudicatorsInTourney)
        {
            subject = "Welcome To NDO 2024, Please see the necessary links for our Tabs";
            body = $"Hello {adjudicator.adjudicatorName},\n\n" +
                          $"You have been successfully registered as an adjudicator for {AppConstants.instance.selectedTouranment.tournamentName}. We are pleased to host you.\n\n" +
                          $"Kindly verify your information:\n" +
                          $"Name: {adjudicator.adjudicatorName}\n" +
                          $"Institute: {AppConstants.instance.GetInstituitionsFromID(adjudicator.instituitionID).instituitionName}\n" +
                          $"Contact: {adjudicator.adjudicatorPhone}\n\n" +
                          $"For any corrections, Kindly Contact: Ali Abdullah, +92 319 2619774\n" +
                          $"Please see the following link for our tabs site: {link}\n\n" +
                          $"Thank You.\n\n" +
                          $"This is an auto generated email, sent from tabX a tab app made by some very nutty people.";
            SendEmail(adjudicator.adjudicatorEmail, subject, body);
        }
    }
    public void SendEmailForAdjudicatorFeedback()
    {
        foreach (var adjudicator in AppConstants.instance.selectedTouranment.adjudicatorsInTourney)
        {
            subject = "Feedback for NDO 2024";
            body = $"Hello {adjudicator.adjudicatorName},\n\n" +
                          $"We hope you had a great round at {AppConstants.instance.selectedTouranment.tournamentName}. We would love to hear your feedback.\n\n" +
                          $"Please fill out the following form: {link}\n\n" +
                          $"Thank You.\n\n" +
                          $"This is an auto generated email, sent from tabX a tab app made by some very nutty people.";
            SendEmail(adjudicator.adjudicatorEmail, subject, body);
        }
    }
    private void SendEmail(string toEmail, string subject, string body)
    {
        try
        {
            SmtpClient client = new SmtpClient(smtpServer, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body
            };
            mailMessage.To.Add(toEmail);

            client.Send(mailMessage);
            Debug.Log($"Email sent successfully to {toEmail}.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to send email to {toEmail}: {ex.Message}");
        }
    }
}