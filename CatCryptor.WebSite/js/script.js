const encryptLoadingAnimation = document.querySelector('.encrypt-loading-animation');
const decryptLoadingAnimation = document.querySelector('.decrypt-loading-animation');
const verticalPathTitleText = "0x1223 ██████████████████ Cat:\\Cryptor\\";

let encryptAnimationInterval;
let decryptAnimationInterval;

document.querySelectorAll(".encrypting-file-drop-input").forEach((inputElement) => {
  const fileDropPanel = inputElement.closest(".file-drop-panel");

  fileDropPanel.addEventListener("click", (e) => {
    inputElement.click();
  });

  inputElement.addEventListener("change", (e) => {
    if (inputElement.files.length) {
      updateThumbnail(fileDropPanel, inputElement.files[0]);
    }
  });

  fileDropPanel.addEventListener("dragover", (e) => {
    e.preventDefault();
    fileDropPanel.classList.add("file-drop-panel-over");
  });

  ["dragleave", "dragend"].forEach((type) => {
    fileDropPanel.addEventListener(type, (e) => {
      fileDropPanel.classList.remove("file-drop-panel-over");
    });
  });

  fileDropPanel.addEventListener("drop", (e) => {
    e.preventDefault();

    if (e.dataTransfer.files.length) {
      inputElement.files = e.dataTransfer.files;
      updateThumbnail(fileDropPanel, e.dataTransfer.files[0]);
    }

    fileDropPanel.classList.remove("file-drop-panel-over");
  });
});

document.querySelectorAll(".decrypting-file-drop-input").forEach((inputElement) => {
  const fileDropPanel = inputElement.closest(".file-drop-panel");

  fileDropPanel.addEventListener("click", (e) => {
    inputElement.click();
  });

  inputElement.addEventListener("change", (e) => {
    if (inputElement.files.length) {
      updateThumbnail(fileDropPanel, inputElement.files[0]);
    }
  });

  fileDropPanel.addEventListener("dragover", (e) => {
    e.preventDefault();
    fileDropPanel.classList.add("file-drop-panel-over");
  });

  ["dragleave", "dragend"].forEach((type) => {
    fileDropPanel.addEventListener(type, (e) => {
      fileDropPanel.classList.remove("file-drop-panel-over");
    });
  });

  fileDropPanel.addEventListener("drop", (e) => {
    e.preventDefault();

    if (e.dataTransfer.files.length) {
      inputElement.files = e.dataTransfer.files;
      updateThumbnail(fileDropPanel, e.dataTransfer.files[0]);
    }

    fileDropPanel.classList.remove("file-drop-panel-over");
  });
});

function onEncryptTabClick() {
    toggleButton('encrypt');
    updateVerticalPathTitle("Encrypt");

    document.getElementById('encrypt-tab').style.display = 'block';
    document.getElementById('decrypt-tab').style.display = 'none';
}

function onDecryptTabClick() {
    toggleButton('decrypt');
    updateVerticalPathTitle("Decrypt");

    document.getElementById('encrypt-tab').style.display = 'none';
    document.getElementById('decrypt-tab').style.display = 'block';
}

async function onExecuteEncryptingClick() {
  startAnimation(encryptLoadingAnimation);
  logInfo("Start encrypting a file...");

  const password = document.getElementById('encrypt-password-input').value;
  const confirmPassword = document.getElementById('confirm-password-input').value;

  if (password.trim() === '' || password !== confirmPassword) {
    logError("Passwords do not match.");
    stopAnimation(encryptLoadingAnimation, encryptAnimationInterval);
    return;
  }

  const fileInput = document.querySelector('.encrypting-file-drop-input');
  const file = fileInput.files[0];
  
  if (!file) {
    logError("Please select a file for encryption.");
    stopAnimation(encryptLoadingAnimation, encryptAnimationInterval);
    return;
  }
  
  const formData = new FormData();
  formData.append('Password', password);
  formData.append('File', file);

  try {
    const response = await fetch('https://localhost:7197/encrypt', {
        method: 'POST',
        body: formData
    });

    if (response.ok) {
      const downloadLink = document.createElement('a');
      downloadLink.href = URL.createObjectURL(await response.blob());
      downloadLink.download = `${file.name}.crpt`;
      downloadLink.click();
      logSuccess("The file is successfully encrypted.");
    } else {
      logError("File encryption failed.");
    }
  } catch (error) {
    logError(`File encryption failed: (${error}).`);
  } finally {
    stopAnimation(encryptLoadingAnimation, encryptAnimationInterval);
  }
}

async function onExecuteDecryptingClick() {
  startAnimation(decryptLoadingAnimation);
  logInfo("Start decrypting a file...");

  const password = document.getElementById('decrypt-password-input').value;

  if (password.trim() === '') {
    logError("Passwords is empty.");
    stopAnimation(decryptLoadingAnimation, decryptAnimationInterval);
    return;
  }

  const fileInput = document.querySelector('.decrypting-file-drop-input');
  const file = fileInput.files[0];
  
  if (!file) {
    logError("Please select a file for encryption.");
    stopAnimation(decryptLoadingAnimation, decryptAnimationInterval);
    return;
  }

  const formData = new FormData();
  formData.append('Password', password);
  formData.append('File', file);

  try {
    const response = await fetch('https://localhost:7197/decrypt', {
      method: 'POST',
      body: formData
    });

    if (response.ok) {
      const downloadLink = document.createElement('a');
      downloadLink.href = URL.createObjectURL(await response.blob());
      downloadLink.download = `${file.name.replace('.crpt', '')}`;
      downloadLink.click();
      logSuccess("The file is successfully decrypted.");
    } else {
      logError("File decrypting failed.");
    }
  } catch (error) {
    logError(`File decrypting failed (${error}).`);
  } finally {
    stopAnimation(decryptLoadingAnimation, decryptAnimationInterval);
  }
}

function startAnimation(loadingAnimation) {
  let animationIndex = 0;

  loadingAnimation.style.display = 'inline-block';
  const animationInterval = setInterval(() => {
    const animationChars = ['-','\\','|','/'];
    loadingAnimation.textContent = animationChars[animationIndex];
    animationIndex = (animationIndex + 1) % animationChars.length;
  }, 200);

  if (loadingAnimation.classList.contains('encrypt-loading-animation')) {
    encryptAnimationInterval = animationInterval;
  } else if (loadingAnimation.classList.contains('decrypt-loading-animation')) {
    decryptAnimationInterval = animationInterval;
  }
}

function stopAnimation(loadingAnimation, animationInterval) {
  clearInterval(animationInterval);
  loadingAnimation.style.display = 'none';
}

function logError(message) {
  const logTitleElement = document.querySelector('.log-title');

  logTitleElement.style.color = '#e3776f';
  logTitleElement.textContent = `cat:\\cryptor> [err] ${message}`;
}

function logInfo(message) {
  const logTitleElement = document.querySelector('.log-title');

  logTitleElement.style.color = '#b3b3b3';
  logTitleElement.textContent = `cat:\\cryptor> [inf] ${message}`;
}

function logSuccess(message) {
  const logTitleElement = document.querySelector('.log-title');

  logTitleElement.style.color = '#6fc0e3';
  logTitleElement.textContent = `cat:\\cryptor> [sccs] ${message}`;
}

function toggleButton(operation) {
    const encryptButton = document.querySelector('.encrypt-button');
    const decryptButton = document.querySelector('.decrypt-button');

    if (operation === 'encrypt') {
        encryptButton.style.backgroundColor = 'white';
        encryptButton.style.color = '#1f1f1f';

        decryptButton.style.backgroundColor = '#1f1f1f';
        decryptButton.style.color = 'white';
    } else if (operation === 'decrypt') {
        encryptButton.style.backgroundColor = '#1f1f1f';
        encryptButton.style.color = 'white';

        decryptButton.style.backgroundColor = 'white';
        decryptButton.style.color = '#1f1f1f';
    }
}

function updateVerticalPathTitle(sectionName) {
  document.getElementsByClassName("vertical-path-title")[0].innerText = verticalPathTitleText + sectionName;
}

function updateThumbnail(fileDropPanel, file) {
  let thumbnailElement = fileDropPanel.querySelector(".file-drop-panel-thumb");

  let imgElement = fileDropPanel.querySelector("img");
  if (imgElement) {
      imgElement.remove();
  }

  if (fileDropPanel.querySelector(".file-drop-title")) {
    fileDropPanel.querySelector(".file-drop-title").remove();
  }

  if (!thumbnailElement) {
    thumbnailElement = document.createElement("div");
    thumbnailElement.classList.add("file-drop-panel-thumb");
    fileDropPanel.appendChild(thumbnailElement);
  }

  thumbnailElement.dataset.label = file.name;

  if (file.type.startsWith("image/")) {
    const reader = new FileReader();

    reader.readAsDataURL(file);
    reader.onload = () => {
      thumbnailElement.style.backgroundImage = `url('${reader.result}')`;
    };
  } else {
    thumbnailElement.style.backgroundImage = null;
  }

  logInfo(`File: ${file.name} Size: ${(file.size / 1024 / 1024).toFixed(2)}MB is ready.`);
}