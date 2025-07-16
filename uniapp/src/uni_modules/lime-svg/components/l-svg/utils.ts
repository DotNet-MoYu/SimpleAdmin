// @ts-nocheck
// #ifdef APP-ANDROID || APP-IOS 
import { fileToDataURL } from '@/uni_modules/lime-file-utils'
// #endif


/**
 * 小程序把路径转成base64
 * @param {string} path 
 * @return 表示 SVG 的 Data URL。
 */
export function pathToDataUrl(path : string) : Promise<string> {
	
	return new Promise((resolve, reject) => {
		// #ifdef MP
		uni.getFileSystemManager().readFile({
			filePath: path,
			encoding: 'base64',
			success: (res) => {
				resolve(`data:image/svg+xml;base64,${res.data}`)
			},
			fail: (error) => {
				console.error({ error, path })
				reject(error)
			}
		})
		// #endif
		// #ifdef APP-ANDROID || APP-IOS 
		const url = fileToDataURL(path)
		if(url == null) {
			reject('路径错误')
		}
		resolve(url!.replace(/\s+/g,''))
		// #endif
		// #ifdef APP-NVUE
		let localFilePath = plus.io.convertAbsoluteFileSystem(path)
		if (localFilePath == path) {
			localFilePath = '_www/' + path.slice(1)
		}
		plus.io.resolveLocalFileSystemURL(localFilePath, (entry) => {
			entry.file((file : any) => {
				const fileReader = new plus.io.FileReader()
				fileReader.onload = (data) => {
					resolve(data.target.result)
				}
				fileReader.onerror = (error) => {
					console.error({ error, path })
					reject(error)
				}
				fileReader.readAsDataURL(file)
			}, reject)
		}, reject)
		// #endif
		// #ifndef APP-ANDROID || APP-IOS || MP || APP-NVUE
		reject('不支持')
		// #endif
	})

}

/**
 * 将 SVG 字符串转换为 Data URL。
 * @param {string} svg - 要转换的 SVG 字符串。
 * @returns {string} 表示 SVG 的 Data URL。
 */
export function svgToDataUrl(svgString : string) : string {
	const encodedSvg = encodeURIComponent(svgString)!.replace(/\+/g, '%20');
	return `data:image/svg+xml,${encodedSvg}`
}