// 动态添加/删除“联系方式”行，并维护 index 以便 ASP.NET Core 模型绑定
const methodTypes = ["电话", "邮箱", "社交账号", "地址", "其他"];

function buildMethodRow(index, type, value) {
    type = type || "电话";
    value = value || "";
    const options = methodTypes.map(t =>
        `<option value="${t}" ${t === type ? "selected" : ""}>${t}</option>`
    ).join("");

    const div = document.createElement("div");
    div.className = "method-row row g-2 mb-2";
    div.innerHTML = `
        <div class="col-md-4">
            <select name="ContactMethods[${index}].Type" class="form-select">${options}</select>
        </div>
        <div class="col-md-6">
            <input name="ContactMethods[${index}].Value" value="${value}" class="form-control" placeholder="请输入内容" />
        </div>
        <div class="col-md-2">
            <button type="button" class="btn btn-outline-danger w-100" onclick="removeMethodRow(this)">删除</button>
        </div>`;
    return div;
}

function addMethodRow() {
    const container = document.getElementById("methodsContainer");
    container.appendChild(buildMethodRow(container.children.length));
}

function removeMethodRow(btn) {
    btn.closest(".method-row").remove();
    reindexMethods();
}

function reindexMethods() {
    const container = document.getElementById("methodsContainer");
    container.querySelectorAll(".method-row").forEach((row, i) => {
        row.querySelector("select").name = `ContactMethods[${i}].Type`;
        row.querySelector("input").name = `ContactMethods[${i}].Value`;
    });
}
