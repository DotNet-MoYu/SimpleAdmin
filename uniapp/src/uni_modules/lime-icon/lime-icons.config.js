// lime.config.js
module.exports = {
	// 输入的文件目录，自有的SVG，如果没有则不需要
	input: {
		prefix: "my-icons",
		dir: '/static/svg',
	},
	// 输出的配置
	output: {
		// 输出的文件目录
		dir: '/static/icon',
		// 输出的文件的格式，如果是JSON则是一个图标合集
		// file: 'icons.json',
		// 如果是SVG则是每个图标做为单独的文件
		file: '*.svg',
	},
	icons: [
		'el:address-book', 
		'uil:12-plus',
		'icon-park-outline:abdominal',
		'icon-park-outline:acoustic'
	]
}