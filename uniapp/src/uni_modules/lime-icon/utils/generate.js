const path = require('path');
const fs = require("fs");
const rootPath = process.cwd(); // 获取根目录
const { importDirectory, blankIconSet } = require("@iconify/tools");
const { locate } = require('@iconify/json');
const { getIconData } = require('@iconify/utils');
const { encodeSvg, saveFile, customOptions, deleteDirectory } = require('./index.js')
async function fetchIconsData(icons) {
	const collections = {}
	for (const iconName of icons) {
		const [collectionName, iconNameWithoutPrefix] = iconName.split(':');
		const filename = locate(collectionName)
		if(!fs.existsSync(filename)) {
			continue
		}
		const icons = JSON.parse(fs.readFileSync(filename, 'utf8'))
		if(!icons) {
			continue 
		}
		if(collectionName && iconNameWithoutPrefix) {
			const iconData = getIconData(icons, iconNameWithoutPrefix);
			if(iconData) {
				if(!collections[collectionName]) {
					collections[collectionName] = blankIconSet(collectionName);
				}
				collections[collectionName].setIcon(iconNameWithoutPrefix, iconData);
			} else {
				console.log(`Icon '${iconName}' not found in '${collectionName}' collection.`)
			}
		} else if(collectionName) {
			if(!collections[collectionName]) {
				collections[collectionName] = blankIconSet(collectionName)
			} 
			Object.keys(icons.icons).forEach(iconName => {
				const iconData = getIconData(icons, iconName)
				if(iconData) {
					collections[collectionName].setIcon(iconName, iconData)
				} else {
					console.log(`Icon '${iconName}' not found in '${collectionName}' collection.`)
				}
			})
		}
	}
	return collections;
}

async function generate(config){
	try {
		if(!config) {
			// 从配置文件中读取选项
			const rootConfigPath = path.join(rootPath, 'lime-icons.config.js');
			let configPath = ''
			if(fs.existsSync(rootConfigPath)) {
				configPath = rootConfigPath
			} else {
				configPath = path.dirname(__filename) + '/lime-icons.config.js'; // 配置文件路径
			}
			
			const configFile = fs.readFileSync(configPath, 'utf8');
			config = eval(`(${configFile})`);
		}
		
		// 根据配置文件中的字段设置选项
		const options = {
			input: Object.assign({}, customOptions, config.input || {}), // 输入的文件目录
			output: {
				dir: config.output.dir || '/static', // 输出的文件目录
				file: config.output.file || 'icons.json', // 输出的文件的格式，默认为 JSON
			},
			icons: config.icons || [], // 图标名称列表
		};
		
		// 先删除原来的
		deleteDirectory(options.output.dir)
		// 处理输入目录的逻辑
		if (config.input.dir.startsWith('/')) {
			options.input.dir = path.join(rootPath, config.input.dir);
		} else if (config.input.dir.startsWith('./')) {
			options.input.dir = path.join(__dirname, config.input.dir.slice(2));
		}
		let iconCollections = {}
		// 异步地从目录中导入图标
		if(fs.existsSync(options.input.dir)) {
			const iconSet = await importDirectory(options.input.dir, options.input);
			// 导出为 JSON 文件
			iconCollections[options.input.prefix] = iconSet
		}
		
		// 获取指定图标的数据
		if(options.icons.length) {
			const iconCollection = await fetchIconsData(options.icons);
			Object.assign(iconCollections, iconCollection)
		}
		
		if(/\.json$/i.test(options.output.file)) {
			const collections = {}
			Object.values(iconCollections).forEach((iconSet) => {
				
				iconSet.forEach(iconName => {
					// 将 SVG 转换为 Data URL
					collections[iconSet.prefix + ':' + iconName] = `data:image/svg+xml;utf8,${encodeSvg(iconSet.toString(iconName))}`
				})
			})
			await saveFile(`${options.output.dir}/${options.output.file}`, JSON.stringify(collections))
		} else {
			Object.values(iconCollections).forEach((iconSet) => {
				iconSet.forEach(async iconName => {
					await saveFile(`${options.output.dir}/${iconSet.prefix}/${iconName}.svg`, iconSet.toString(iconName))
				})
			})
		}
	} catch (error) {
		console.error("导出图标集为 JSON 文件时出错：", error);
	}
}


module.exports = {
	generate
}