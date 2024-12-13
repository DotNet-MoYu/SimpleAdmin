/**
 * @description mqtt 消息订阅
 * @license Apache License Version 2.0
2022
,
 * @Copyright (c) 2022-Now 少林寺驻北固山办事处大神父王喇嘛
 * @remarks
 * SimpleAdmin 基于 Apache License Version 2.0 协议发布，可用于商业项目，但必须遵守以下补充条款:
 * 1.请不要删除和修改根目录下的LICENSE文件。
 * 2.请不要删除和修改SimpleAdmin源码头部的版权声明。
 * 3.分发源码时候，请注明软件出处 https://gitee.com/dotnetmoyu/SimpleAdmin
 * 4.基于本软件的作品，只能使用 SimpleAdmin 作为后台服务，除外情况不可商用且不允许二次分发或开源。
 * 5.请不得将本软件应用于危害国家安全、荣誉和利益的行为，不能以任何形式用于非法为目的的行为不要删除和修改作者声明。
 * 6.任何基于本软件而产生的一切法律纠纷和责任，均于我司无关
 */
import mqtt from "mqtt";
import { defineStore } from "pinia";
import { mqttApi } from "@/api";
import { useMessageStore } from "@/stores/modules";
import { MqttMessage, MsgTypeEnum } from "../interface/mqtt";

const name = "simple-mqtt"; // 定义模块名称

/* MqttState */
export interface MqttState {
  /** 服务端地址 */
  url: string | undefined;
  /** 是否清除 */
  clean: boolean;
  /** 客户端 */
  client: any;
  /** 客户端id */
  clientId: string;
  /** 订阅主题列表 */
  topics: string[];
  /** 订阅主题的集合，key为topic, value为接收到该topic时需要执行的回调 */
  subscribeMembers: [{ string: any }] | undefined;
}

/** 订阅参数 */
export interface subscribeParams {
  /** 订阅主题 */
  topic: string;
  /** 订阅回调 */
  callback: number;
  /** 订阅选项 */
  subscribeOption: any;
}

/** Mqtt模块 */
export const useMqttStore = defineStore({
  id: name,
  state: (): MqttState => ({
    url: undefined,
    client: undefined,
    clientId: "",
    clean: true,
    topics: [],
    subscribeMembers: undefined
  }),
  getters: {},
  actions: {
    /** 获取mqtt参数 */
    async getMqttParameter() {
      return await mqttApi.getMqttParameter().then(res => {
        console.log("[ res ] >", res);
        this.clientId = res.data.clientId;
        this.url = res.data.url;
        this.topics = res.data.topics;
        return res.data;
      });
    },
    /** 初始化mqtt客户端 */
    async initMqttClient() {
      console.log("[ mqtt初始化 ] >");
      if (this.client) {
        this.disconnect();
      }
      await this.getMqttParameter().then(res => {
        const url = this.url;
        const options = {
          username: res.userName,
          password: res.password,
          clientId: this.clientId,
          clean: true, // true: 清除会话, false: 保留会话
          connectTimeout: 4000, // 超时时间
          keepAlive: 60 // 心跳时间
        };
        console.log("[ options ] >", options);
        // return;
        if (url) {
          this.client = mqtt.connect(url, options);
          this.client.on("connect", e => {
            this.onConnect(e);
          });
          this.client.on("reconnect", err => {
            this.onReconnect(err);
          });
          this.client.on("error", err => {
            this.onError(err);
          });
          this.client.on("message", (topic, message) => {
            this.onMessage(topic, message);
          });
        }
      });
    },
    /** 断开连接 */
    disconnect() {
      this.client.end();
      this.client = undefined;
      this.subscribeMembers = {};
    },
    /** 订阅 */
    subscribe(topics: string[]) {
      topics.forEach((item: string) => {
        this.client.subscribe(item, {}, (err, res) => {
          if (err) {
            console.log(`客户端: ${this.clientId}, 订阅主题: ${item}失败: `, err);
          } else {
            console.log(`客户端: ${this.clientId}, 订阅主题: ${item}成功,${JSON.stringify(res)}`);
          }
        });
      });
    },
    /** 订阅带回调函数 */
    subscribeCallback(params: subscribeParams) {
      const { topic, callback, subscribeOption } = params;
      this.client.subscribe(topic, subscribeOption, (err, res) => {
        if (err) {
          console.log(`客户端: ${this.clientId}, 订阅主题: ${topic}失败: `, err);
        } else {
          console.log(`客户端: ${this.clientId}, 订阅主题: ${topic}成功,${res}`);
        }
      });
      this.subscribeMembers.push({ topic, callback });
    },
    /** 取消订阅 */
    unsubscribe(topic: string) {
      if (!this.client) return;
      this.client.unsubscribe(topic, {}, (err, res) => {
        if (err) {
          console.log(`客户端: ${this.clientId}, 取消订阅主题: ${topic}失败: `, err);
        } else {
          console.log(`客户端: ${this.clientId}, 取消订阅主题: ${topic}成功,${res}`);
        }
      });
      this.subscribeMembers = this.subscribeMembers.filter((item: any) => item.topic !== topic);
    },
    /** 连接事件 */
    onConnect(e: any) {
      console.log(`客户端: ${this.clientId}, 连接mqtt服务器成功:`, e);
      this.subscribe(this.topics);
    },
    /** 重连事件 */
    onReconnect(err: any) {
      console.log(`客户端: ${this.clientId}, 正在重连mqtt服务器...`, err);
    },
    /** 错误事件 */
    onError(err: any) {
      console.log(`客户端: ${this.clientId}, 连接mqtt服务器失败:`, err);
    },
    /** 消息事件 */
    onMessage(topic: string, message: any) {
      console.log(message.toString());
      const messageStore = useMessageStore();
      const msg = JSON.parse(message.toString()) as MqttMessage;
      console.log(`客户端: ${this.clientId}, 接收到消息:`, topic, msg);
      switch (msg.MsgType) {
        case MsgTypeEnum.NewMessage:
          messageStore.getNewMessage(true, msg.Data.Subject);
          break;
        case MsgTypeEnum.LoginOut:
          break;
        case MsgTypeEnum.UpdatePassword:
          break;
      }
    }
  }
});
