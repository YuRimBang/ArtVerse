using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using TMPro;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;

public class SendEmail : MonoBehaviour
{
    public GameObject emailButton;
    public GameObject mainUI;

    public Canvas selectFileCanvas;
    public Canvas InputEmailCanvas;
    public Canvas ErrorCanvas;
    public Canvas SendCheckCanvas;
    public Canvas aftersendCanvas;

    public TMP_InputField email;
    public TMP_Dropdown emailAddress;

    public TMP_Text errorText;
    public TMP_Text sendInfoCheckText;

    private List<string> userSelectedFileList;

    void Start()
    {
        selectFileCanvas.gameObject.SetActive(false);
        InputEmailCanvas.gameObject.SetActive(false);
        ErrorCanvas.gameObject.SetActive(false);
        SendCheckCanvas.gameObject.SetActive(false);
        aftersendCanvas.gameObject.SetActive(false);
    }

    public void viewInputUserInfoUI()
    {
        userSelectedFileList = emailButton.GetComponent<SelectFile>().getUserSelectedFiles();
    }

    public void onBack_InputEmailCanvasButton()
    {
        InputEmailCanvas.gameObject.SetActive(false);
        selectFileCanvas.gameObject.SetActive(true);
    }

    public void OnCloseButton()
    {
        userSelectedFileList = null;
        InputEmailCanvas.gameObject.SetActive(false);
        mainUI.SetActive(true);
    }

    public void OnSend_InputEmailCanvasButton()
    {
        sendPreparation();
    }

    public void OnBackButton_ErrorCanvasButton()
    {
        ErrorCanvas.gameObject.SetActive(false);
        InputEmailCanvas.gameObject.SetActive(true);
    }

    public void OnRightButton()
    {
        send();
    }

    public void OnNoButton()
    {
        SendCheckCanvas.gameObject.SetActive(false);
        InputEmailCanvas.gameObject.SetActive(true);
    }

    public void OncloseButton_AfterSendCanvas()
    {
        aftersendCanvas.gameObject.SetActive(false);
        mainUI.SetActive(true);
    }

    private bool IsValidEmailAndPassword(string email)
    {
        string emailPattern = @"^[a-zA-Z0-9]+$";
        string passwordPattern = @"^[a-zA-Z0-9!@#$%^&*(),.?{}|<>-_=+~`]+$";

        if (!Regex.IsMatch(email, emailPattern))
        {
            return false;
        }
        return true;
    }

    private void sendPreparation()
    {
        string userEmail = email.text.Trim();
        //userPassword = password.text.Trim();

        if (string.IsNullOrEmpty(userEmail))
        {
            errorText.text = "이메일을 입력하지 않았어요ㅠㅠ";
            InputEmailCanvas.gameObject.SetActive(false);
            ErrorCanvas.gameObject.SetActive(true);
        }

        else if (!IsValidEmailAndPassword(userEmail))
        {
            errorText.text = "올바르지 않은 이메일 형식입니다ㅠㅠ";
            InputEmailCanvas.gameObject.SetActive(false);
            ErrorCanvas.gameObject.SetActive(true);
        }

        else
        {
            sendInfoCheckText.text = $"[발송 정보]\n\n이메일: {userEmail + emailAddress.options[emailAddress.value].text}\n선택한 파일: {userSelectedFileList.Count}개";
            InputEmailCanvas.gameObject.SetActive(false);
            SendCheckCanvas.gameObject.SetActive(true);
        }
    }

    private void send()
    {
        try
        {
            //네이버 메일 계정
            string sendEmail = "leey511@naver.com";
            string sendPw = "artbus0000";

            string userEmail = email.text.Trim() + emailAddress.options[emailAddress.value].text;

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(sendEmail); // 발신자 이메일 설정
            mail.To.Add(userEmail);              // 수신자 이메일 설정
            mail.Subject = "[ArtVerse] 작가님의 작품이 도착했어요!";
            mail.Body = "작가님의 멋진 작품을 보냅니다. 앞으로도 멋진 작품 활동 부탁드려요!";
            mail.IsBodyHtml = false;

            // 첨부 파일 추가
            foreach (string filePath in userSelectedFileList)
            {
                if (System.IO.File.Exists(filePath))
                {
                    Attachment attachment = new Attachment(filePath);
                    mail.Attachments.Add(attachment);
                }
            }   

            // SMTP 설정
            SmtpClient smtpServer = new SmtpClient("smtp.naver.com");
            smtpServer.Port = 587; // 네이버 SMTP 포트
            smtpServer.Credentials = new NetworkCredential(sendEmail, sendPw);
            smtpServer.EnableSsl = true; // SSL 활성화

            // 이메일 전송
            smtpServer.Send(mail);

            userSelectedFileList = null;
            SendCheckCanvas.gameObject.SetActive(false);
            aftersendCanvas.gameObject.SetActive(true);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error sending email: {ex.Message}");
        }
    }
}
