export type CreateAnimationOptions = {
	/**
	 * 动画持续时间，单位ms
	 */
	duration ?: number;
	/**
	 * 定义动画的效果
	 * - linear: 动画从头到尾的速度是相同的
	 * - ease: 动画以低速开始，然后加快，在结束前变慢
	 * - ease-in: 动画以低速开始
	 * - ease-in-out: 动画以低速开始和结束
	 * - ease-out: 动画以低速结束
	 * - step-start: 动画第一帧就跳至结束状态直到结束
	 * - step-end: 动画一直保持开始状态，最后一帧跳到结束状态
	 */
	timingFunction ?: string //'linear' | 'ease' | 'ease-in' | 'ease-in-out' | 'ease-out' | 'step-start' | 'step-end';
	/**
	 * 动画延迟时间，单位 ms
	 */
	delay ?: number;
	/**
	 * 设置transform-origin
	 */
	transformOrigin ?: string;
}