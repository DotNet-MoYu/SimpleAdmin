<template>
	<text class="l-icon" :class="[classes]" :style="[styles, lStyle]" v-if="!isImage && !isIconify && !isSVG" @click="$emit('click')">{{iconCode}}</text>
	<image class="l-icon" :class="[classes]" :style="[styles, lStyle]" v-else-if="(!isSVG && !isIconify) && isImage" :src="iconUrl" @click="$emit('click')"></image>
	<l-svg class="l-icon" :class="[classes]" :style="[styles, lStyle]" v-else :web="web" :color="color" :src="iconUrl"  @error="imageError" @load="imageLoad" @click="$emit('click')"></l-svg>
</template>
<script lang="ts">
	// @ts-nocheck
	/**
	 * LimeIcon 图标
	 * @description ICON集
	 * @tutorial https://ext.dcloud.net.cn/plugin?id=14057
	 * @property {String} name 图标名称
	 * @property {String} color 颜色
	 * @property {String} size 尺寸
	 * @property {String} prefix 字体图标前缀  
	 * @property {Boolean} inherit 是否继承颜色 
	 * @property {Boolean} web 原生 app(nvue,uvue) 是否使用web渲染  
	 * @event {Function} click 点击事件
	 */
	// @ts-nocheck
	import { computed, defineComponent, ref, inject } from '@/uni_modules/lime-shared/vue';
	import icons from '../../static/icons.json';
	import { addUnit } from '@/uni_modules/lime-shared/addUnit';
	
	import { isObject } from '@/uni_modules/lime-shared/isObject';
	import IconProps from './props';
	
	// #ifdef VUE2 && MP
	import { getClassStr } from '@/uni_modules/lime-shared/getClassStr';
	// #endif
	
	// #ifdef APP-NVUE
	import iconSrc from '@/uni_modules/lime-icon/hybrid/html/t3.ttf';
	var domModule = weex.requireModule('dom');
	domModule.addRule('fontFace', {
		'fontFamily': "uniicons",
		'src': "url('" + iconSrc + "')"
	});
	// #endif
	
	const name = 'l-icon';
	export default defineComponent({
		name,
		externalClasses: ['l-class'],
		options: {
			addGlobalClass: true,
			virtualHost: true,
		},
		props: IconProps,
		emits: ['click'],
		setup(props, { emit }) {
			const $iconCollection = inject('$iconCollection', null)
			const { $limeIconsHost: $iconsHost } = uni as any
			const IconifyURL = 'https://api.iconify.design/'
			// const isAPP = uni.getSystemInfoSync().uniPlatform == 'app'
			const innerName = computed(():string => props.name || '')
			const hasHost = computed(() => `${innerName.value}`.indexOf('/') !== -1);
			const isIconify = computed(() => !hasHost.value && `${innerName.value}`.includes(':'))
			const collectionIcon = computed(() => isObject($iconCollection) && $iconCollection.icons[innerName.value])
			const isImage = computed<boolean>(() : boolean => {
				return /\.(jpe?g|png|gif|bmp|webp|tiff?)$/i.test(innerName.value) || /^data:image\/(jpeg|png|gif|bmp|webp|tiff);base64,/.test(innerName.value)
			})
			const isSVG = computed<boolean>(() : boolean => {
				return /\.svg$/i.test(innerName.value) || innerName.value.startsWith('data:image/svg+xml') || innerName.value.startsWith('<svg')
			})
			const classes = computed(() => {
				const { prefix } = props
				const iconPrefix = prefix || name
				const iconName = `${iconPrefix}-${innerName.value}`
				const isFont = !isImage.value && !isIconify.value && !isSVG.value
				const isImages = isImage.value || isIconify.value || isSVG.value
				const cls = {
					[iconPrefix]: !isImages && prefix,
					[iconName]: !isImages,
					[`${name}--image`]: isImages,
					[`${name}--font`]: isFont,
					// [`is-inherit`]: isIconify.value && (props.color || props.inherit)
				}
				
				// #ifdef VUE2 && MP
				return getClassStr(cls)
				// #endif
				
				return cls
			})
			const iconCode = computed(() => {
				const isImages = isImage.value || isIconify.value || isSVG.value
				return (!isImages && icons[innerName.value]) || (/[^\x00-\x7F]/.test(innerName.value) ? innerName.value : '')
			})
			const isError = ref(false)
			const cacheMap = new Map<string, string>()
			const iconUrl = computed(() => {
				const hasIconsHost = $iconsHost != null && $iconsHost != '' 
				// const hasIconCollection = $iconCollectiont != null
				if (isImage.value) {
					return hasHost.value ? innerName.value : ($iconsHost || '') + innerName.value
				} else if (isIconify.value) {
					// 防止重绘
					if(cacheMap.has(innerName.value) && !isError.value) {
						return cacheMap.get(innerName.value)!
					}
					// 如果存在collectionIcon则使用 
					// 如果设置的路径加载失败 就使用网络地址 就使用iconify api
					// !isError.value && 
					const _host = `${hasIconsHost ? $iconsHost : IconifyURL}`
					const _icon = collectionIcon.value || _host + `${innerName.value}.svg`.replace(/:/g, '/')
					cacheMap.set(innerName.value, _icon)
					return _icon
				} else if (isSVG.value) {
					return (/\.svg$/i.test(innerName.value) && hasIconsHost && !hasHost.value ? $iconsHost : '') + innerName.value.replace(/'/g, '"')
				} else {
					return null
				}
			})
			const styles = computed(() => {
				const style : Record<string, any> = {
					'color': props.color,
				}
				if (typeof props.size == 'number' || props.size) {
					style['font-size'] = addUnit(props.size)
				}
				//#ifdef VUE2
				// VUE2小程序最后一个值莫名的出现undefined
				style['undefined'] = ''
				// #endif
				return style
			})
			const imageLoad = () => {
				isError.value = false
			}
			const imageError = () => {
				isError.value = true
			}

			return {
				iconCode,
				classes,
				styles,
				isImage,
				isSVG,
				isIconify,
				iconUrl,
				imageLoad,
				imageError
			}
		}
	})
</script>
<style lang="scss">
	@import './index.scss';
</style>