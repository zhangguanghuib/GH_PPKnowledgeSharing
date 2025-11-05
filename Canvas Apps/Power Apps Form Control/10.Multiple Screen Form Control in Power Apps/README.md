# 10.Multiple Screen Form Control in Power Apps

1. How this apps works? <br/>
   The home screen looks like<br/>
   <img width="579" height="892" alt="image" src="https://github.com/user-attachments/assets/7f4bc272-0ec1-47a1-a13d-f1202b662b33" /><br/><hr/>
   The "+" button code <br/>
   

   ```
     ResetForm(BasicInforForm);
     ResetForm(DetailsForm);
     ResetForm(AdditionalInfoForm);
     Set(
         varFormData,
         Defaults('Work tracker01')
     );
     Set(
         varDisplayMode,
          FormMode.Edit
     );
     Navigate('Basic Form Info')
   ```

The view button code is <br/>

```
   ResetForm(BasicInforForm);
   ResetForm(DetailsForm);
   ResetForm(AdditionalInfoForm);
   Set(
       varFormData,
       ThisItem
   );
   Set(
       varDisplayMode,
       FormMode.View
   );
   Navigate('Basic Form Info')
```

The Edit Icon Code is <br/>

```
   ResetForm(BasicInforForm);
   ResetForm(DetailsForm);
   ResetForm(AdditionalInfoForm);
   Set(
       varFormData,
       ThisItem
   );
   Set(
       varDisplayMode,
       FormMode.Edit
   );
   Navigate('Basic Form Info')
```

2. When click "+" to create a new record<br/>
   <img width="819" height="914" alt="image" src="https://github.com/user-attachments/assets/2059b28d-40bd-4a95-8c2d-c158dcadf5b3" /><br/><hr/>
   The design is as this <br/><hr/>
   <img width="1672" height="1228" alt="image" src="https://github.com/user-attachments/assets/cbfb7083-0097-4b08-a7d6-5dd756a31561" /><br/><hr/>

3. The details form is as <br/><hr/>
    <img width="1725" height="1250" alt="image" src="https://github.com/user-attachments/assets/ceb08154-0f1c-499c-86ac-af22a0a67d4a" /><br/><hr/>

4. Additional information form looks like <br/>
   <img width="1653" height="1228" alt="image" src="https://github.com/user-attachments/assets/a741b750-5010-4601-bc31-4316f3d0e764" /><br/><hr/>

   The Submit button code is:<br/><hr/>

   <img width="2493" height="933" alt="image" src="https://github.com/user-attachments/assets/00098f4c-f65d-4c6a-b571-dbadf73a02a8" /><br/><hr/>

