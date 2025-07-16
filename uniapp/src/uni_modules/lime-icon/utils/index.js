const fs = require("fs");
const glob = require('glob');
const path = require("path");
const rootPath = process.cwd(); // 获取根目录
// https://bl.ocks.org/jennyknuth/222825e315d45a738ed9d6e04c7a88d0
function encodeSvg(svg) {
	return svg
		.replace(
			"<svg",
			~svg.indexOf("xmlns") ? "<svg" : '<svg xmlns="http://www.w3.org/2000/svg"'
		)
		.replace(/"/g, "'")
		.replace(/%/g, "%25")
		.replace(/#/g, "%23")
		.replace(/{/g, "%7B")
		.replace(/}/g, "%7D")
		.replace(/</g, "%3C")
		.replace(/>/g, "%3E");
}


function isDirectoryEmpty(path) {
	const files = fs.readdirSync(path);
	return files.length === 0;
  }
function deleteFolderBFS(folderPath) {
	const outputPath = /^\.|\/|\\/.test(folderPath)  ? path.join(rootPath, folderPath): folderPath
	if(!fs.existsSync(outputPath)) {
		return
	}
	const queue = [outputPath];
	while (queue.length > 0) {
		const currentPath = queue.shift();
		const currentStats = fs.statSync(currentPath);

		if (currentStats.isDirectory()) {
			const files = fs.readdirSync(currentPath);
			for (const file of files) {
				const filePath = path.join(currentPath, file);
				const fileStats = fs.statSync(filePath);
				if (fileStats.isDirectory()) {
					queue.push(filePath);
				} else {
					fs.unlinkSync(filePath); // 删除文件
				}
			}
			if(isDirectoryEmpty(currentPath)) {
				fs.rmdirSync(currentPath);
			}
		} 
	}
}

// 保存
async function saveFile(file, data) {
	const outputPath = /^(\.|\/|\\)/.test(file) ? path.join(rootPath, file) : file;
	const outputDir = path.dirname(outputPath);
	try {
		// 创建文件夹
		await fs.promises.mkdir(outputDir, {
			recursive: true
		});

		// 使用 Promise 进行写入文件操作
		await fs.promises.writeFile(outputPath, data, "utf8");
		// console.log(`成功保存文件：${outputPath}`);
	} catch (error) {
		console.error("保存文件时出错:", error);
	}
}

// 可选的选项对象
const customOptions = {
	prefix: "l", // 为图标集设置前缀
	includeSubDirs: true, // 启用扫描子目录中的文件（默认启用）
	keyword: (fileName, defaultKeyword, iconSet) => {
		// 根据文件名自定义关键字生成
		// 返回关键字或 undefined 以跳过该文件
		return defaultKeyword;
	},
	ignoreImportErrors: true, // 禁用未成功导入图标时的错误抛出（默认启用）
	keepTitles: false, // 禁用在 SVG 中保留标题（默认禁用）
};
module.exports = {
	encodeSvg,
	saveFile,
	deleteDirectory: deleteFolderBFS,
	customOptions,
};