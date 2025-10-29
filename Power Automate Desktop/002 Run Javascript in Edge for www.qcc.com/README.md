# Run Javascript in Edge for www.qcc.com

## The requirement is to 

Go to https://www.qcc.com/firm/ce37fdaaa0def14412bfb9d40c0f5a3b.html, and then click "报告" button  <br/>
<img width="2882" height="1593" alt="image" src="https://github.com/user-attachments/assets/29af6567-094c-4c97-8e94-1758593779b6" /><br/><hr/>

If that is the paid user, the button should be "下载"(download), otherwise it shows the "开通VIP"

<img width="1773" height="1604" alt="image" src="https://github.com/user-attachments/assets/4a1bfe8a-c486-4e62-a950-cb4674ccba5a" /><br/><hr/>

click button "下载" to download the report <br/>


## Solution:

1. Declare an Input Variable <br/>

<img width="2298" height="1331" alt="image" src="https://github.com/user-attachments/assets/f94849f6-26ae-4366-bcdd-74f437c98837" /><br/><hr/>

2. Launch browser and attached to the running instance <br/>

<img width="962" height="765" alt="image" src="https://github.com/user-attachments/assets/3df92978-fdd4-404d-b41f-03e6391bc95e" /><br/><hr/>

3. Click "报告" button<br/><hr/>
<img width="2175" height="1427" alt="image" src="https://github.com/user-attachments/assets/1020eabc-4669-4821-9579-eea296075ef7" /><br/><hr/>

to open this page:<br/>

4.  Find "企业信用报告-专业版" and then find the "download or open VIP" and click it <br/>
<img width="1832" height="1685" alt="image" src="https://github.com/user-attachments/assets/a3f0fa79-f039-455d-b4c3-83226f4e871e" /><br/><hr/>

The javascript code is as below <br/>
```js
function ExecuteScript() { 
	// 目标文本
	//const targetText = "企业信用报告-专业版";
	const targetText = %reportNameToDownload%;
	// 查找文本所在的元素
	let targetElement = null;
	const elements = document.querySelectorAll('div');
	for (const el of elements) {
	    if (el.textContent.trim() === targetText) {
	        targetElement = el;
	    }
	}
	
	if (targetElement) {
	    // 向上查找最近的 .content 容器
	    const contentDiv = targetElement.closest('.content');
	    if (contentDiv) {
	        // 在该容器内查找开通VIP按钮
	        const vipButton = contentDiv.querySelector('.btn.btn-primary');
	        if (vipButton) {
	            // 模拟点击
	            vipButton.click();
	            console.log('已点击 "下载" 按钮');
	        } else {
	            console.log('未找到 "下载" 按钮');
	        }
	    } else {
	        console.log('未找到 .content 容器');
	    }
	} else {
	    console.log('未找到包含 "' + targetText + '" 的元素');
	}
}
```
