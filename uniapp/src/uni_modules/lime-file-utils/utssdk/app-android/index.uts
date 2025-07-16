import Base64 from "android.util.Base64";
import MimeTypeMap from "android.webkit.MimeTypeMap";
import ByteArrayOutputStream from 'java.io.ByteArrayOutputStream';

import File from "java.io.File";
import FileInputStream from "java.io.FileInputStream";
import FileOutputStream from "java.io.FileOutputStream";
import InputStream from 'java.io.InputStream';

// import IOException from "java.io.IOException";
import { ProcessFileOptions, NullableString } from '../interface'
type NullByteArray = ByteArray | null

function inputStreamToArray(inputStream : InputStream) : NullByteArray {
	try {
		let bos : ByteArrayOutputStream = new ByteArrayOutputStream()
		let bytes : ByteArray = new ByteArray(1024)

		do {
			let length = inputStream.read(bytes)
			if (length != -1) {
				bos.write(bytes, 0, length)
			} else {
				break
			}
		} while (true)
		bos.close()
		return bos.toByteArray()
	} catch (e : Throwable) {
		return null;
	}
}

function getMimeType(filePath : string) : NullableString {
	const extension = MimeTypeMap.getFileExtensionFromUrl(filePath);
	if (extension == null) return null
	return MimeTypeMap.getSingleton().getMimeTypeFromExtension(extension);
}

export function getResourcePath(path : string) : string | null {
	let uri = path
	if (uri.startsWith("http") || uri.startsWith("<svg") || uri.startsWith("data:image/")) {
		return uri
	}
	if (uri.startsWith("file://")) {
		uri = uri.substring("file://".length)
	} else if (uri.startsWith("unifile://")) {
		uri = UTSAndroid.convert2AbsFullPath(uri)
	} else {
		uri = UTSAndroid.convert2AbsFullPath(uri)
		if (uri.startsWith("/android_asset/")) {
			try {
				const context = UTSAndroid.getUniActivity()!;
				const inputStream = context.getResources()!.getAssets().open(path.replace('/android_asset/', ''))
				inputStream.close();
				return uri
			} catch (e) {
				return null
			}
		}
	}
	const file = new File(uri)
	if (file.exists()) {
		return uri
	}
	return null
}


/**
 * 检查路径存在性及类型 (Android 实现)
 * @param path 要检查的完整路径（支持内部存储和外部存储路径）
 * @return Pair<是否存在, 是否是目录>
 */
export function checkExistence(filePath : string):boolean[] {
	const path = getResourcePath(filePath)
	if(path == null) return [false, false]
	const file = new File(path)
	const exists = file.exists()
	
	if(exists) {
		return [true, file.isDirectory]
	} else {
		return [false, false]
	}
}

/**
 * 检查路径是否存在
 * @param path 要检查的完整路径
 */
export function isExists(filePath : string):boolean {
	const result = checkExistence(filePath);
	return result[0]
}

/**
 * 检查路径是否是存在的目录
 * @param path 要检查的完整路径
 */
export function isDirectory(filePath : string):boolean {
	const result = checkExistence(filePath);
	return result[0] && result[1]
}

/**
 * 检查指定路径是否为存在的文件
 * @param path 要检查的完整路径
 * @return 当且仅当路径存在且是普通文件时返回 true
 */
export function isFile(filePath : string):boolean {
	const result = checkExistence(filePath);
	return result[0] && !result[1]
}



export function fileToBase64(filePath : string) : NullableString {
	try {
		const context = UTSAndroid.getUniActivity()!;
		let path = filePath;
		let imageBytes : NullByteArray = null

		if (path.startsWith("file://")) {
			path = path.replace("file://", "")
		} else {
			// if(!path.startsWith("/storage") && !path.startsWith("/android_asset/"))
			// path = UTSAndroid.getResourcePath(path)
			path = UTSAndroid.convert2AbsFullPath(path)
		}

		if (path.startsWith("/android_asset/")) {
			imageBytes = inputStreamToArray(context.getResources()!.getAssets().open(path.replace('/android_asset/', '')))
		} else {
			const file = new File(path)
			if (file.exists()) {
				let fis : FileInputStream = new FileInputStream(file);
				imageBytes = inputStreamToArray(fis);
				fis.close();
			}
		}
		if (imageBytes == null) return null
		return Base64.encodeToString(imageBytes, Base64.DEFAULT)
	} catch (e) {
		return null
	}
}
export function fileToDataURL(filePath : string) : NullableString {
	const base64 = fileToBase64(filePath)
	const mimeType = getMimeType(filePath);
	if (base64 == null || mimeType == null) return null;
	return "data:" + mimeType + ";base64," + base64;
}


function getFileExtensionFromDataURL(dataURL : string) : string {
	const commaIndex = dataURL.indexOf(",");
	const mimeType = dataURL.substring(0, commaIndex).replace("data:", "").replace(";base64", "");
	const mimeTypeParts = mimeType.split("/");
	return mimeTypeParts[1];
}
function dataURLToBytes(dataURL : string) : ByteArray {
	const commaIndex = dataURL.indexOf(",");
	const base64 = dataURL.substring(commaIndex + 1);
	return Base64.decode(base64, Base64.DEFAULT);
}

export function dataURLToFile(dataURL : string, filename : NullableString = null) : NullableString {
	try {
		const bytes = dataURLToBytes(dataURL);
		const name = filename ?? `${Date.now()}.${getFileExtensionFromDataURL(dataURL)}`;
		const cacheDir = UTSAndroid.getAppCachePath()!;
		const destFile = new File(cacheDir, name);
		const path = new File(cacheDir); 
		if(!path.exists()){
			path.mkdir(); 
		}
		const fos = new FileOutputStream(destFile)
		fos.write(bytes);
		fos.close();
		return `${cacheDir}${name}`
	} catch (e) {
		console.error('dataURLToFile::', e)
		return null
	}
}


// function requestSystemPermission(fun:()=> void) {
// 	let permissionNeed = ["android.permission.WRITE_EXTERNAL_STORAGE"]
// 	UTSAndroid.requestSystemPermission(UTSAndroid.getUniActivity()!, permissionNeed, function (allRight : boolean, _ : string[]) {
// 		if (allRight) {
// 			// 权限请求成功
// 			console.log(`allRight`, allRight)
// 			fun()
// 		} else {
// 			//用户拒绝了部分权限
// 		}
// 	}, function (_ : boolean, _ : string[]) {
// 		//用户拒绝了部分权限
// 	})
// }


export function processFile(options : ProcessFileOptions) {

	if (options.type == 'toBase64') {
		const res = fileToBase64(options.path)
		const err = 'fileToBase64: 解析失败'
		if (res != null) {
			options.success?.(res)
			options.complete?.(res)
		} else {
			options.complete?.(err)
			options.fail?.(err)
		}
	} else if (options.type == 'toDataURL') {
		const res = fileToDataURL(options.path)
		const err = 'fileToDataURL: 解析失败'
		if (res != null) {
			options.success?.(res)
			options.complete?.(res)
		} else {
			options.complete?.(err)
			options.fail?.(err)
		}
	} else if (options.type == 'toFile') {
		const res = dataURLToFile(options.path, options.filename)
		const err = 'dataURLToFile: 解析失败'
		if (res != null) {
			options.success?.(res)
			options.complete?.(res)
		} else {
			options.complete?.(err)
			options.fail?.(err)
		}
	}
}