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
