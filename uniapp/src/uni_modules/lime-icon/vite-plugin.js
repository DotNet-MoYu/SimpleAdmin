const { readFileSync, existsSync } = require('fs');
const path = require('path');
const {generate} = require('./utils/generate')

// 插件的名称
const pluginName = 'vite-plugin-limeIcon';

// 要监听的组件的名称
const targetComponent = 'l-icon';

function parseAttributes(attributesStr) {
	if (!attributesStr.includes("'")) {
		return [attributesStr]
	}
	const regex = /'([^']+)'/g;
	const matches = attributesStr.match(regex);
	if (matches) {
		const targetContent = matches.map(item => item.replace(/'/g, ''));
		return targetContent
	} else {
		return [attributesStr]
	}
}

function extractAttributes(content) {
	const regex = /<l-icon\s*[^>]*(:?)name=["]([^"]+)["][^>]*>/g; // /<l-icon\s+(.*?)\s*\/?>/g //<l-icon\s+([^>]+)\s*\/?>/g;
	let attributes = [];
	const attributesSet = new Set(attributes);
	let match;
	while ((match = regex.exec(content)) !== null) {
		const attributesStr = match[2];
		const attributesList = parseAttributes(attributesStr);
		for (const attribute of attributesList) {
		  attributesSet.add(attribute); // 添加新属性到Set中
		}
	}
	attributes = [...attributesSet];
	return attributes;
}
// 遍历每个文件并检查是否使用了目标组件
let iconCollections = {}
let files = {}
let timer = null
function processFile(file, options) {
	const filePath = path.resolve(file);
	const content = readFileSync(filePath, 'utf-8');

	// 检查文件是否包含目标组件
	if (content.includes(targetComponent) && (!file.includes('l-icon.vue') || !file.includes('l-icon.uvue')) && files[file] !== content) {
		const icons = extractAttributes(content)
		if(icons && icons.length) {
			files[file] = content
			iconCollections[file] = icons
		}
		Object.values(iconCollections).forEach(icons => {
			if(options.icons) {
				options.icons = options.icons.concat(icons);
			} else {
				options.icons = icons
			}
		}) 
		
		clearTimeout(timer)
		timer = setTimeout(() => {
			options.icons = Array.from(new Set(options.icons))
			generate(options)
		},500)
		
	}
}

// 插件的钩子函数
function vitePlugin(options = {}) {
	const {useInDevelopment = false} = options
	const isDev = process.env.NODE_ENV === 'development'
	return {
		name: pluginName,
		transform(code, id) {
			if (id.endsWith('.vue') && (useInDevelopment && isDev || !useInDevelopment && !isDev)) {
				// 处理Vue文件
				processFile(id, options);
			}
		},
	};
}

module.exports = vitePlugin;