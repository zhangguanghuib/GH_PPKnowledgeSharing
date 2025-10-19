# Run Javascript
1. Launch browser:<br/>
<img width="927" height="743" alt="image" src="https://github.com/user-attachments/assets/6b756e32-cfa9-40ae-b202-8578a4250dc4" /><br/>
<img width="897" height="616" alt="image" src="https://github.com/user-attachments/assets/95245647-126b-4e98-8137-b3686ad65da9" /><br/>
<img width="969" height="454" alt="image" src="https://github.com/user-attachments/assets/44579808-998c-4844-953a-7b2be302766f" /><br/>
<img width="925" height="632" alt="image" src="https://github.com/user-attachments/assets/a06b3e30-6789-4997-8d1c-98b9a191086c" /><br/>

2. For the last step, the html section is like:<br/>
```
<div class="input-group-prepend">
  <div class="input-group-text font-weight-bolder">案件說明</div>
</div>
<textarea readonly="readonly" rows="3" wrap="soft" class="form-control form-control-lg" style="resize: none;" id="__BVID__2681"></textarea>
<!-- -->
```
```js
function ExecuteScript() {
  const textAreaElement = Array.from(document.querySelectorAll(".input-group-text"))
    .find((el) => el.innerText.trim() === "案件編號")
    ?.parentElement
    ?.nextElementSibling;
    
    return textAreaElement._value;
}
```
