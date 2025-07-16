<template>
	<!-- #ifdef APP-NVUE -->
	<web-view class="l-svg" ref="webRef" @error="error" @pagefinish="loaded" @onPostMessage="message"
		src="/uni_modules/lime-svg/hybrid/html/index.html"></web-view>
	<!-- <l-svg-x v-else :src="src" :color="color" @load="onLoad" @error="onError"></l-svg-x> -->
	<!-- #endif -->

	<!-- #ifndef APP-NVUE -->
	<view class="l-svg" :class="{'is-inherit': isInherit}" :style="[styles]" @click="$emit('click')">
		<image class="l-svg-img" :src="path" @load="onLoad" @error="onError"></image>
	</view>
	<!-- #endif -->
</template>

<script lang="ts">
	// @ts-nocheck
	/**
	 * Svg SVG组件
	 * @description 用于渲染SVG路径元素，支持动态颜色和继承属性
	 * <br>插件类型：LSvpComponentPublicInstance 
	 * @tutorial https://ext.dcloud.net.cn/plugin?name=lime-svg
	 * 
	 * @property {string} src SVG路径
	 * @property {string} color 路径颜色（默认："currentColor"）
	 * @property {boolean} web 是否启用Web优化模式（默认：false）
	 * @property {boolean} inherit 是否继承父级SVG属性（默认：true）
	 * @event {Function} load SVG路径加载完成时触发
	 * @event {Function} error SVG路径加载失败时触发
	 */
	import svpProps from './props'
	import { defineComponent, ref, computed, watchEffect, getCurrentInstance } from '@/uni_modules/lime-shared/vue'
	import { pathToDataUrl, svgToDataUrl } from './utils'
	
	export default defineComponent({
		// name: 'l-svg',
		props: svpProps,
		emits: ['load', 'error', 'click'],
		setup(props, { emit }) {
			const path = ref('')
			// #ifndef APP-NVUE
			const isInherit = computed(() : boolean => {
				return props.color != ''
			})
			// #endif
			const formatUrl = (url: string, action:string):string => {
				if(url.indexOf(`'`) > 0) return `${action}("${url}")`
				return `${action}('${url}')`
			}
			const instance = getCurrentInstance()!.proxy!
			const imageURL = ref(null)
			const styles = computed(() => {
				const style: Record<string, any>  = {}
				// #ifndef APP-NVUE
				if (path.value != '') {
					const image = formatUrl(imageURL.value || path.value, 'url')// + ';'
					if (isInherit.value) {
						style['-webkit-mask-image'] =  image
						style['mask-image'] = image
					} else {
						style['background-image'] =  image
					}
				}
				// #endif
				if (props.color != '') {
					style['color'] = props.color
				}
				//#ifdef VUE2
				// VUE2小程序最后一个值莫名的出现undefined
				style['undefined'] = ''
				// #endif
				return style
			})

			const onLoad = (e) => {
				// #ifdef WEB
				imageURL.value = instance.$el.querySelector('img').src
				// #endif
				emit('load')
			}

			const onError = () => {
				emit('error')
			}
			
			// APP
			// #ifdef APP-NVUE
			const webRef = ref(null)
			const setSvgSrc = () => {
				if (path.value != '') {
					webRef.value?.evalJS(formatUrl(path.value, 'setSrc'));
				}
			}
			const setSvgColor = () => {
				if (props.color != '' && path.value != '') {
					webRef.value?.evalJS(`setStyle({"--color": "${props.color}"})`);
				}
			}
			const error = () => {
				emit('error')
			}
			const loaded = () => {
				watchEffect(() => {
					if (props.src == '') return
					if (props.src.startsWith('<')) {
						path.value = props.src
					} else if (props.src.startsWith('/static')) {
						pathToDataUrl(props.src).then(res => {
							path.value = res;
							setSvgSrc()
							setSvgColor()
						}).catch(err => {
							emit('error')
							console.warn("[lime-svg]" + props.src + JSON.stringify(err))
						})
					} else {
						path.value = props.src
					}
					setSvgSrc()
					setSvgColor()
				})
			}
			const message = (e) => {
				const data = e.detail.data[0]
				const event = data?.event
				const eventDetaill = data?.data
				if (event == 'click') {
					emit('click', eventDetaill)
				} else if (event == 'load') {
					emit('load', eventDetaill)
				} else if (event == 'error') {
					emit('error', eventDetaill)
				}
			}
			// #endif

			// 小程序 WEB
			// #ifndef APP-NVUE
			watchEffect(() => {
				if (props.src == '') return
				if (props.src.startsWith('<svg')) {
					path.value = svgToDataUrl(props.src)
				} else if (props.src.startsWith('/static')) {
					// #ifdef WEB
					path.value = props.src
					// #endif
					// #ifdef APP-VUE
					path.value = props.src.slice(1)
					// #endif
					// #ifndef WEB || APP-VUE
					pathToDataUrl(props.src).then(res => {
						path.value = res;
					}).catch(err => {
						emit('error')
						console.warn("[lime-svg]" + props.src + JSON.stringify(err))
					})
					// #endif
				} else {
					path.value = props.src
				}
			})
			// #endif


			return {
				path,
				onLoad,
				onError,
				// #ifdef APP-NVUE
				webRef,
				error,
				loaded,
				message,
				// #endif
				// #ifndef APP-NVUE
				isInherit,
				styles
				// #endif
			}
		}
	})
</script>

<style lang="scss">
	/* #ifndef APP-NVUE */
	:host {
		display: inline-flex;
	}
	/* #endif */
	.l-svg {
		/* #ifdef APP-NVUE */
		flex: 1;
		/* #endif */
		/* #ifndef APP-NVUE */
		display: inline-flex;
		width: 100%;
		height: 100%;
		flex: 1;
		/* #endif */
		
		/* #ifndef APP-NVUE */
		:deep(svg) {
			width: 100%;
			height: 100%;
		}

		&-img {
			mix-blend-mode: lighten;
			width: 100%;
			height: 100%;
		}

		&.is-inherit {
			// background-image: var(--svg);//linear-gradient(#f09, #09f, #f0f);
			// background-blend-mode: lighten;
			// background-size: cover;

			// background-size: 100% 100%;
			// background-repeat: no-repeat;
			// -webkit-mask-image: var(--svg);
			// mask-image: var(--svg);
			-webkit-mask-repeat: no-repeat;
			mask-repeat: no-repeat;
			-webkit-mask-size: 100% 100%;
			mask-size: 100% 100%;
			// -webkit-mask-mode: alpha;
			// mask-mode: alpha;
			background-color: currentColor; //var(currentColor, transparent);
			// background-blend-mode: multiply;
		}

		&:not(.is-inherit) {
			// background: var(--svg) no-repeat;
			background-repeat: no-repeat;
			background-size: 100% 100%;
			background-color: transparent;
			image {
				mix-blend-mode: inherit;
				opacity: 0;
			}
		}
		/* #endif */
		
	}
</style>